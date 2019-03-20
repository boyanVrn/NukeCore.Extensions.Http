using System.Collections.Generic;
using System.Linq;

namespace UCS.Extensions.Http.Sender.Entities
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
    }
}