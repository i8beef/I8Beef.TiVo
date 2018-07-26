using System;
using System.Text.RegularExpressions;

namespace I8Beef.TiVo.Commands
{
    /// <summary>
    /// TiVo TELEPORT command class.
    /// </summary>
    public class TeleportCommand : Command
    {
        /// <inheritdoc/>
        public override string Code { get { return "TELEPORT"; } }

        /// <summary>
        /// Keyboard code to send.
        /// </summary>
        public string TeleportCode { get; set; }

        /// <summary>
        /// Parses a commands string to return an instance of this <see cref="Command"/>.
        /// </summary>
        /// <param name="commandString">The command string to parse.</param>
        /// <returns>The <see cref="Command"/>.</returns>
        public static Command Parse(string commandString)
        {
            var matches = Regex.Match(commandString, @"^TELEPORT (.*)$");
            if (!matches.Success)
                throw new ArgumentException("Command string not recognized: " + commandString);

            var teleportCode = matches.Groups[1].Value;

            return new TeleportCommand { TeleportCode = teleportCode };
        }

        /// <inheritdoc/>
        public override string GetTelnetCommand()
        {
            return $"{Code} {TeleportCode}";
        }
    }
}
