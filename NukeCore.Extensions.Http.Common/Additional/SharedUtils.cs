using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NukeCore.Extensions.Http.Common.Additional
{
    public static class SharedUtils
    {
        public static T AsEnum<T>(this int value, T def) where T : struct, Enum => Enum.IsDefined(typeof(T), value) ? (T)(object)value : def;
        public static T AsEnum<T>(this string value) where T : struct, Enum => AsEnum(value, default(T));
        public static T AsEnum<T>(this string value, T def) where T : struct, Enum
            => Enum.IsDefined(typeof(T), value) ? Enum.TryParse<T>(value, ignoreCase: true, out var res) ? res : def : def;

        public static bool In<T>(this T val, params T[] values) where T : Enum => values.Contains(val);
        public static bool NotIn<T>(this T val, params T[] values) where T : Enum => !values.Contains(val);
        public static byte[] ToUTF8ByteArray(this string value) => string.IsNullOrEmpty(value) ? new byte[] { } : Encoding.UTF8.GetBytes(value);

        public static async Task<byte[]> ToByteArrayAsync(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                return ms.ToArray();
            }
        }

        public static string ToStringUtf8(this byte[] bytes) => bytes.Length >= 0 ? Encoding.UTF8.GetString(bytes) : string.Empty;

        public static async Task<string> ToStringUtf8Async(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                var arr = ms.ToArray();
                return arr.ToStringUtf8();
            }
        }

        public static string ToStringSql(this DateTime src) => src.ToString("yyyy-MM-dd HH:mm:ss");


        public static T ActivateObject<T>()
        {
            if (typeof(T).IsValueType || typeof(T) == typeof(string))
            {
                return default;
            }

            return (T)Activator.CreateInstance(typeof(T));
        }
    }
}
