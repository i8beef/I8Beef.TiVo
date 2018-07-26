namespace I8Beef.TiVo.Commands
{
    /// <summary>
    /// Command factory class.
    /// </summary>
    public static class CommandFactory
    {
        /// <summary>
        /// Gets the <see cref="Command"/> encoded by given command string.
        /// </summary>
        /// <param name="commandString">The command string to parse.</param>
        /// <returns>The <see cref="Command"/> encoded by given command string.</returns>
        public static Command GetCommand(string commandString)
        {
            if (commandString.StartsWith("IRCODE"))
                return IrCommand.Parse(commandString);

            if (commandString.StartsWith("KEYBOARD"))
                return KeyboardCommand.Parse(commandString);

            if (commandString.StartsWith("SETCH"))
                return SetChCommand.Parse(commandString);

            if (commandString.StartsWith("FORCECH"))
                return ForceChCommand.Parse(commandString);

            if (commandString.StartsWith("TELEPORT"))
                return TeleportCommand.Parse(commandString);

            return null;
        }
    }
}
