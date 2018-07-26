using System;
using System.Text.RegularExpressions;

namespace I8Beef.TiVo.Responses
{
    /// <summary>
    /// TiVo CH_STATUS command class.
    /// </summary>
    public class ChannelStatusResponse : Response
    {
        /// <inheritdoc/>
        public override string InResponseToCode { get { return "SETCH"; } }

        /// <inheritdoc/>
        public override string Code { get { return "CH_STATUS"; } }

        /// <summary>
        /// Parses a commands string to return an instance of this <see cref="Response"/>.
        /// </summary>
        /// <param name="responseString">The response string to parse.</param>
        /// <returns>The <see cref="Response"/>.</returns>
        public static Response Parse(string responseString)
        {
            var matches = Regex.Match(responseString, @"^CH_STATUS (.*)$");
            if (!matches.Success)
                throw new ArgumentException("Response string not recognized: " + responseString);

            var value = matches.Groups[1].Value;

            return new ChannelStatusResponse { Value = value };
        }
    }
}
