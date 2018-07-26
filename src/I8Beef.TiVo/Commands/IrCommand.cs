﻿using System;
using System.Text.RegularExpressions;

namespace I8Beef.TiVo.Commands
{
    /// <summary>
    /// TiVo IRCODE command class.
    /// </summary>
    public class IrCommand : Command
    {
        /// <inheritdoc/>
        public override string Code { get { return "IRCODE"; } }

        /// <summary>
        /// Parses a commands string to return an instance of this <see cref="Command"/>.
        /// </summary>
        /// <param name="commandString">The command string to parse.</param>
        /// <returns>The <see cref="Command"/>.</returns>
        public static Command Parse(string commandString)
        {
            var matches = Regex.Match(commandString, @"^IRCODE (.*)$");
            if (!matches.Success)
                throw new ArgumentException("Command string not recognized: " + commandString);

            var value = matches.Groups[1].Value;

            return new IrCommand { Value = value };
        }

        /// <inheritdoc/>
        public override string GetTelnetCommand()
        {
            return $"{Code} {Value}";
        }
    }
}
