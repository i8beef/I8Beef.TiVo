using I8Beef.TiVo.Commands;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace I8Beef.TiVo.TestClient
{
    /// <summary>
    /// Main entry point for test program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main entry point for test program.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main()
        {
            var host = "";

            using (var client = new Client(host, 31339))
            {
                client.MessageSent += (o, e) => { Debug.WriteLine("Message sent: " + e.Message); };
                client.MessageReceived += (o, e) => { Debug.WriteLine("Message received: " + e.Message); };

                client.EventReceived += (o, e) => { Console.WriteLine($"Event received: {e.GetType()} {e.Response.Code} {e.Response.Value}"); };

                client.Connect();

                var commandString = string.Empty;
                while (commandString != "exit")
                {
                    if (!string.IsNullOrEmpty(commandString))
                    {
                        var command = CommandFactory.GetCommand(commandString);
                        if (command != null)
                        {
                            if (commandString.StartsWith("SETCH"))
                            {
                                var response = await client.SendQueryAsync(command);
                                Console.WriteLine($"New value for {response.GetType()}: {response.Value}");
                            }
                            else
                            {
                                await client.SendCommandAsync(command);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Command not recognized");
                        }
                    }

                    Console.Write("Enter command: ");
                    commandString = Console.ReadLine();
                }
            }
        }
    }
}
