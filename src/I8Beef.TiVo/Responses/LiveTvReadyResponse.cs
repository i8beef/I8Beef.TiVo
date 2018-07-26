using System;
using System.Text.RegularExpressions;

namespace I8Beef.TiVo.Responses
{
    /// <summary>
    /// TiVo LIVETV_READY command class.
    /// </summary>
    public class LiveTvReadyResponse : Response
    {
        /// <inheritdoc/>
        public override string InResponseToCode { get { return "TELEPORT"; } }

        /// <inheritdoc/>
        public override string Code { get { return "LIVETV_READY"; } }

        /// <summary>
        /// Parses a commands string to return an instance of this <see cref="Response"/>.
        /// </summary>
        /// <param name="responseString">The response string to parse.</param>
        /// <returns>The <see cref="Response"/>.</returns>
        public static Response Parse(string responseString)
        {
            var matches = Regex.Match(responseString, @"^LIVETV_READY$");
            if (!matches.Success)
                throw new ArgumentException("Response string not recognized: " + responseString);

            var value = matches.Groups[1].Value;

            return new LiveTvReadyResponse { Value = value };
        }
    }
}
