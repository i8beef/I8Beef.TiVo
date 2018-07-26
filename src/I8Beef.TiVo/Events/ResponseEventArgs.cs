using System;
using I8Beef.TiVo.Responses;

namespace I8Beef.TiVo.Events
{
    /// <summary>
    /// Response event args class.
    /// </summary>
    public class ResponseEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseEventArgs"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        public ResponseEventArgs(Response response)
        {
            Response = response;
        }

        /// <summary>
        /// The <see cref="Response"/>.
        /// </summary>
        public Response Response { get; private set; }
    }
}
