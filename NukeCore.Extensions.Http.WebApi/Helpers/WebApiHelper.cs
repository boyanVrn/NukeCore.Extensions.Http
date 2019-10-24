using System.Text;

namespace NukeCore.Extensions.Http.WebApi.Helpers
{
    public static class WebApiHelper
    {
        public static byte[] ToUTF8ByteArray(this string value) => string.IsNullOrEmpty(value) ? new byte[] { } : Encoding.UTF8.GetBytes(value);
    }
}
