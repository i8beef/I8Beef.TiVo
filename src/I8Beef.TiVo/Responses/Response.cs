namespace I8Beef.TiVo.Responses
{
    /// <summary>
    /// TiVo Response abstract class.
    /// </summary>
    public abstract class Response
    {
        /// <summary>
        /// TiVo command code this response results from.
        /// </summary>
        public abstract string InResponseToCode { get; }

        /// <summary>
        /// TiVo response code.
        /// </summary>
        public abstract string Code { get; }

        /// <summary>
        /// TiVo response value.
        /// </summary>
        public string Value { get; set; }
    }
}
