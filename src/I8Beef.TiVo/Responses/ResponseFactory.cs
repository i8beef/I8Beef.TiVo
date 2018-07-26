namespace I8Beef.TiVo.Responses
{
    /// <summary>
    /// Response factory class.
    /// </summary>
    public static class ResponseFactory
    {
        /// <summary>
        /// Gets the <see cref="Response"/> encoded by given command string.
        /// </summary>
        /// <param name="responseString">The response string to parse.</param>
        /// <returns>The <see cref="Response"/> encoded by given response string.</returns>
        public static Response GetResponse(string responseString)
        {
            if (responseString.StartsWith("CH_STATUS"))
                return ChannelStatusResponse.Parse(responseString);

            if (responseString.StartsWith("CH_FAILED"))
                return ChannelFailedResponse.Parse(responseString);

            if (responseString.StartsWith("LIVETV_READY"))
                return LiveTvReadyResponse.Parse(responseString);

            if (responseString.StartsWith("MISSING_TELEPORT_NAME"))
                return MissingTeleportNameResponse.Parse(responseString);

            return null;
        }
    }
}
