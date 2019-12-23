using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using I8Beef.TiVo.Commands;
using I8Beef.TiVo.Events;
using I8Beef.TiVo.Exceptions;
using I8Beef.TiVo.Responses;

namespace I8Beef.TiVo
{
    /// <inheritdoc />
    public class Client : IClient
    {
        // A lookup to correlate request and responses
        private readonly IDictionary<string, TaskCompletionSource<Response>> _resultTaskCompletionSources = new ConcurrentDictionary<string, TaskCompletionSource<Response>>();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly object _writeLock = new object();

        private readonly string _host;
        private readonly int _port;

        private bool _disposed;
        private TcpClient _client;
        private NetworkStream _stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="host">Host address for TCP client.</param>
        /// <param name="port">Port for the TCP client.</param>
        public Client(string host, int port = 31339)
        {
            _host = host;
            _port = port;
        }

        /// <inheritdoc />
        public event EventHandler<ErrorEventArgs> Error;

        /// <inheritdoc />
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <inheritdoc />
        public event EventHandler<MessageSentEventArgs> MessageSent;

        /// <inheritdoc />
        public event EventHandler<ResponseEventArgs> EventReceived;

        /// <inheritdoc />
        public async Task SendCommandAsync(Command command)
        {
            await FireAndForgetAsync(command).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Response> SendQueryAsync(Command command)
        {
            return await RequestResponseAsync(command).ConfigureAwait(false);
        }

        /// <summary>
        /// Listens for incoming XML stanzas and raises the appropriate events.
        /// </summary>
        /// <remarks>This runs in the context of a separate thread. In case of an
        /// exception, the Error event is raised and the thread is shutdown.</remarks>
        private void ReadStream()
        {
            using (var reader = new StreamReader(_stream))
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    // If the client is not connected close reader.
                    if (!_client.Connected)
                        break;

                    var message = reader.ReadLineSingleBreak();
                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));

                    // Parse message
                    var response = ResponseFactory.GetResponse(message);
                    if (response != null)
                    {
                        if (_resultTaskCompletionSources.TryGetValue(response.InResponseToCode, out TaskCompletionSource<Response> resultTaskCompletionSource))
                        {
                            // query response
                            resultTaskCompletionSource.TrySetResult(response);
                        }
                        else
                        {
                            // event
                            EventReceived?.Invoke(this, new ResponseEventArgs(response));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Send a message without waiting for response.
        /// </summary>
        /// <param name="command">The message to be sent.</param>
        /// <param name="timeout">Time to wait for an error response.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task FireAndForgetAsync(Command command, int timeout = 50)
        {
            // Heartbeat check
            if (!_client.Connected)
                throw new ConnectionException("Connection already closed or not authenticated");

            // Prepare the TaskCompletionSource, which is used to await the result
            var resultTaskCompletionSource = new TaskCompletionSource<Response>(TaskCreationOptions.RunContinuationsAsynchronously);
            if (!_resultTaskCompletionSources.ContainsKey(command.Code))
            {
                _resultTaskCompletionSources[command.Code] = resultTaskCompletionSource;

                // Start the sending
                Send(command.GetTelnetCommand().ToUpper());

                // Await, to make sure there wasn't an error
                var task = await Task.WhenAny(resultTaskCompletionSource.Task, Task.Delay(timeout)).ConfigureAwait(false);

                // Remove the result task, as we no longer need it.
                _resultTaskCompletionSources.Remove(command.Code);

                // This makes sure the exception, if there was one, is unwrapped
                await task.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Sends a request response message.
        /// </summary>
        /// <param name="command">The message to be sent.</param>
        /// <param name="timeout">Timeout for the response, exception thrown on expiration.</param>
        /// <returns>Response message.</returns>
        private async Task<Response> RequestResponseAsync(Command command, int timeout = 2000)
        {
            if (!_client.Connected)
                throw new ConnectionException("Connection already closed or not authenticated");

            // Prepate the TaskCompletionSource, which is used to await the result
            var resultTaskCompletionSource = new TaskCompletionSource<Response>(TaskCreationOptions.RunContinuationsAsynchronously);
            if (!_resultTaskCompletionSources.ContainsKey(command.Code))
            {
                _resultTaskCompletionSources[command.Code] = resultTaskCompletionSource;

                // Create the action which is called when a timeout occurs
                Action timeoutAction = () =>
                {
                    _resultTaskCompletionSources.Remove(command.Code);
                    resultTaskCompletionSource.TrySetException(new TimeoutException($"Timeout while waiting on response {command.Code} after {timeout}"));
                };

                Send(command.GetTelnetCommand());

                // Handle timeout
                var cancellationTokenSource = new CancellationTokenSource(timeout);
                using (cancellationTokenSource.Token.Register(timeoutAction))
                {
                    // Await until response or timeout
                    var result = await resultTaskCompletionSource.Task.ConfigureAwait(false);

                    // Remove the result task, as we no longer need it.
                    _resultTaskCompletionSources.Remove(command.Code);

                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Sends the specified string to the server.
        /// </summary>
        /// <remarks>
        /// Do not use this method directly except for stream initialization and closing.
        /// </remarks>
        /// <param name="message">The string to send.</param>
        /// <exception cref="ArgumentNullException">The xml parameter is null.</exception>
        /// <exception cref="IOException">There was a failure while writing to the network.</exception>
        private void Send(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            // Attach carriage return character if not present
            if (!message.EndsWith("\r"))
                message += "\r";

            byte[] buf = Encoding.ASCII.GetBytes(message);
            lock (_writeLock)
            {
                try
                {
                    _stream.Write(buf, 0, buf.Length);
                    MessageSent?.Invoke(this, new MessageSentEventArgs(message));
                }
                catch
                {
                    Close();
                    throw;
                }
            }
        }

        #region Connection management

        /// <inheritdoc />
        public void Connect()
        {
            // Establish a connection
            if (_client == null)
                _client = new TcpClient(_host, _port);

            // Setup keepalive
            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            // Get stream handle
            if (_stream == null)
                _stream = _client.GetStream();

            // Set up the listener task after authentication has been handled
            Task.Factory.StartNew(ReadStream, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default)
                .ContinueWith(
                    task =>
                    {
                        Error?.Invoke(this, new ErrorEventArgs(task.Exception));
                        Close();
                        throw task.Exception;
                    }, TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <inheritdoc />
        public void Close()
        {
            Dispose();
        }

        #endregion

        #region IDisposable implementation

        /// <summary>
        /// Releases all resources used by the current instance of the XmppIm class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases all resources used by the current instance of the XmppIm class, optionally disposing of managed resource.
        /// </summary>
        /// <param name="disposing">true to dispose of managed resources, otherwise false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                // Get rid of managed resources.
                if (disposing)
                {
                    // Stop parsing incoming feed
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();

                    // Dispose of stream
                    if (_stream != null)
                        _stream.Dispose();

                    // Disconnect socket
                    if (_client != null)
                    {
                        _client.Close();
                        _client = null;
                    }
                }

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }

        #endregion
    }
}
