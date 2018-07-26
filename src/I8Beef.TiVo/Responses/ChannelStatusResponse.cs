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
        /// Channel.
        /// </summary>
        public int Channel { get; set; }

        /// <summary>
        /// Subchannel.
        /// </summary>
        public int? Subchannel { get; set; }

        /// <summary>
        /// Reason.
        /// </summary>
        public string Reason { get; set; }

        /// <inheritdoc />
        public override string Value { get { return Channel + (Subchannel.HasValue ? $" {Subchannel.Value} " : " ") + Reason; } }

        /// <summary>
        /// Parses a commands string to return an instance of this <see cref="Response"/>.
        /// </summary>
        /// <param name="responseString">The response string to parse.</param>
        /// <returns>The <see cref="Response"/>.</returns>
        public static Response Parse(string responseString)
        {
            var matches = Regex.Match(responseString, @"^CH_STATUS (\d*) (\d*)?\s?(.*)$");
            if (!matches.Success)
                throw new ArgumentException("Response string not recognized: " + responseString);

            var channel = int.Parse(matches.Groups[1].Value);
            int? subchannel = null;
            if (!string.IsNullOrEmpty(matches.Groups[2].Value))
                subchannel = int.Parse(matches.Groups[2].Value);
            var reason = matches.Groups[3].Value;

            return new ChannelStatusResponse { Channel = channel, Subchannel = subchannel, Reason = reason };
        }
    }
}
