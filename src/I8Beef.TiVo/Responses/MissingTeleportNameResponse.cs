using System;
using System.Text.RegularExpressions;

namespace I8Beef.TiVo.Responses
{
    /// <summary>
    /// TiVo MISSING_TELEPORT_NAME command class.
    /// </summary>
    public class MissingTeleportNameResponse : Response
    {
        /// <inheritdoc/>
        public override string InResponseToCode { get { return "TELEPORT"; } }

        /// <inheritdoc/>
        public override string Code { get { return "MISSING_TELEPORT_NAME"; } }

        /// <summary>
        /// Parses a commands string to return an instance of this <see cref="Response"/>.
        /// </summary>
        /// <param name="responseString">The response string to parse.</param>
        /// <returns>The <see cref="Response"/>.</returns>
        public static Response Parse(string responseString)
        {
            var matches = Regex.Match(responseString, @"^MISSING_TELEPORT_NAME");
            if (!matches.Success)
                throw new ArgumentException("Response string not recognized: " + responseString);

            var value = matches.Groups[1].Value;

            return new MissingTeleportNameResponse { Value = value };
        }
    }
}
