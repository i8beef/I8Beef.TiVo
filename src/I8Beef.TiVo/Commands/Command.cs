namespace I8Beef.TiVo.Commands
{
    /// <summary>
    /// TiVo Command abstract class.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// TiVo command code.
        /// </summary>
        public abstract string Code { get; }

        /// <summary>
        /// TiVo command value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets the telnet version of this command.
        /// </summary>
        /// <returns>The telnet version of this command.</returns>
        public abstract string GetTelnetCommand();
    }
}
