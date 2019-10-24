using System.Collections.Generic;

namespace NukeCore.Extensions.Http.Common.Models
{
    public class CustomParams : Dictionary<string, object>
    {
        public void AddOrUpdate(string name, object value)
        {
            if (string.IsNullOrEmpty(name) || value == null) return;
            if (ContainsKey(name)) Remove(name);

            Add(name, value);
        }

        public T GetAsDef<T>(string key, T def = default)
            where T : class
                => TryGetValue(key, out var value) ? value as T ?? def : def;

    }
}