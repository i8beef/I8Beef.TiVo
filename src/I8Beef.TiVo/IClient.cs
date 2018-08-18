using System;
using System.IO;
using System.Threading.Tasks;
using I8Beef.TiVo.Commands;
using I8Beef.TiVo.Events;
using I8Beef.TiVo.Responses;

namespace I8Beef.TiVo
{
    /// <summary>
    /// TiVo client.
    /// </summary>
    public interface IClient : IDisposable
    {
        /// <summary>
        /// The event that is raised when an unrecoverable error condition occurs.
        /// </summary>
        event EventHandler<ErrorEventArgs> Error;

        /// <summary>
        /// The event that is raised when and event is received from the TiVo unit.
        /// </summary>
        event EventHandler<ResponseEventArgs> EventReceived;

        /// <summary>
        /// The event that is raised when messages are received.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// The event that is raised when messages are sent.
        /// </summary>
        event EventHandler<MessageSentEventArgs> MessageSent;

        /// <summary>
        /// Allows for explicit closing of session.
        /// </summary>
        void Close();

        /// <summary>
        /// Connect to TiVo.
        /// </summary>
        void Connect();

        /// <summary>
        /// Send command to the TiVo.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> to send.</param>
        /// <returns>Awaitable Task.</returns>
        Task SendCommandAsync(Command command);

        /// <summary>
        /// Send command to the TiVo.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> to send.</param>
        /// <returns>The response.</returns>
        Task<Response> SendQueryAsync(Command command);
    }
}
