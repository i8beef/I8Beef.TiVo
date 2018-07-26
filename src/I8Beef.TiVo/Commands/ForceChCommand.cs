using System;
using System.Text.RegularExpressions;

namespace I8Beef.TiVo.Commands
{
    /// <summary>
    /// TiVo FORCECH command class.
    /// </summary>
    public class ForceChCommand : Command
    {
        /// <inheritdoc/>
        public override string Code { get { return "FORCECH"; } }

        /// <summary>
        /// Channel.
        /// </summary>
        public int Channel { get; set; }

        /// <summary>
        /// Subchannel.
        /// </summary>
        public int? Subchannel { get; set; }

        /// <summary>
        /// Parses a commands string to return an instance of this <see cref="Command"/>.
        /// </summary>
        /// <param name="commandString">The command string to parse.</param>
        /// <returns>The <see cref="Command"/>.</returns>
        public static Command Parse(string commandString)
        {
            var matches = Regex.Match(commandString, @"^FORCECH (\d*)\s?(\d*)?$");
            if (!matches.Success)
                throw new ArgumentException("Command string not recognized: " + commandString);

            var channel = int.Parse(matches.Groups[1].Value);
            int? subchannel = null;
            if (!string.IsNullOrEmpty(matches.Groups[2].Value))
                subchannel = int.Parse(matches.Groups[2].Value);

            return new ForceChCommand { Channel = channel, Subchannel = subchannel };
        }

        /// <inheritdoc/>
        public override string GetTelnetCommand()
        {
            return $"{Code} {Channel}" + (Subchannel.HasValue ? $" {Subchannel}" : string.Empty);
        }
    }
}
