using System;
using System.Text.RegularExpressions;

namespace I8Beef.TiVo.Responses
{
    /// <summary>
    /// TiVo CH_FAILED command class.
    /// </summary>
    public class ChannelFailedResponse : Response
    {
        /// <inheritdoc/>
        public override string InResponseToCode { get { return "SETCH"; } }

        /// <inheritdoc/>
        public override string Code { get { return "CH_FAILED"; } }

        /// <summary>
        /// Reason for the failure.
        /// </summary>
        public string Reason { get; set; }

        /// <inheritdoc />
        public override string Value { get { return Reason; } }

        /// <summary>
        /// Parses a commands string to return an instance of this <see cref="Response"/>.
        /// </summary>
        /// <param name="responseString">The response string to parse.</param>
        /// <returns>The <see cref="Response"/>.</returns>
        public static Response Parse(string responseString)
        {
            var matches = Regex.Match(responseString, @"^CH_FAILED (.*)$");
            if (!matches.Success)
                throw new ArgumentException("Response string not recognized: " + responseString);

            var reason = matches.Groups[1].Value;

            return new ChannelFailedResponse { Reason = reason };
        }
    }
}
