using System;
using System.Text.RegularExpressions;

namespace I8Beef.TiVo.Commands
{
    /// <summary>
    /// TiVo KEYBOARD command class.
    /// </summary>
    public class KeyboardCommand : Command
    {
        /// <inheritdoc/>
        public override string Code { get { return "KEYBOARD"; } }

        /// <summary>
        /// Keyboard code to send.
        /// </summary>
        public string KeyboardCode { get; set; }

        /// <summary>
        /// Parses a commands string to return an instance of this <see cref="Command"/>.
        /// </summary>
        /// <param name="commandString">The command string to parse.</param>
        /// <returns>The <see cref="Command"/>.</returns>
        public static Command Parse(string commandString)
        {
            var matches = Regex.Match(commandString, @"^KEYBOARD (.*)$");
            if (!matches.Success)
                throw new ArgumentException("Command string not recognized: " + commandString);

            var keyboardCode = matches.Groups[1].Value;

            return new KeyboardCommand { KeyboardCode = keyboardCode };
        }

        /// <inheritdoc/>
        public override string GetTelnetCommand()
        {
            return $"{Code} {KeyboardCode}";
        }
    }
}
