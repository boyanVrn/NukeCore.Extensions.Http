using System;

namespace NukeCore.Extensions.Http.Common.Helpers
{

    /// <summary>
    /// additional static methods
    /// </summary>
    public static class HttpClientHelper
    {
        private const string Slash = "/";

        private static string AddTrailingSlash(string url) => url.EndsWith(Slash) ? url : url + Slash;

        /// <summary>
        /// Add slash in the end of http address string if not exist
        /// </summary>
        /// <param name="src">input address</param>
        /// <returns>modify address </returns>
        public static Uri AppendSlash(Uri src)
        {
            if (src == null) return null;
            return src.AbsoluteUri.EndsWith(Slash) ? src : new Uri(src.AbsoluteUri + Slash);
        }
    }
}
