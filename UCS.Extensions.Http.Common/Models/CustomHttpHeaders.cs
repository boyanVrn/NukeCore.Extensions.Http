using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace UCS.Extensions.Http.Common.Models
{

    public class CustomHttpHeaders : Dictionary<string, List<string>>
    {
        public void AddOrUpdate(string name, string value)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value)) return;
            if (ContainsKey(name)) Remove(name);

            Add(name, new List<string> { value });
        }

        public void AddOrUpdate(string name, List<string> value)
        {
            if (string.IsNullOrEmpty(name) || value == null || !value.Any()) return;
            if (ContainsKey(name)) Remove(name);

            Add(name, value);
        }

        public void CopyTo(HttpRequestHeaders httpRequestHeaders)
        {
            foreach (var header in this)
            {
                httpRequestHeaders.Add(header.Key, header.Value);
            }
        }

        public static CustomHttpHeaders CreateFrom(HttpRequestHeaders headers)
        {
            var result = new CustomHttpHeaders();

            foreach (var header in headers)
            {
                result.AddOrUpdate(header.Key, header.Value.ToList());
            }

            return result;
        }

        public static CustomHttpHeaders CreateFrom(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            var result = new CustomHttpHeaders();

            foreach (var header in headers)
            {
                result.AddOrUpdate(header.Key, header.Value.ToList());
            }

            return result;
        }
    }
}