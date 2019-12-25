using System.Collections.Generic;
using System.Linq;

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

        public void AddOrUpdate(IEnumerable<KeyValuePair<string, object>> paramsCollection)
        {
            foreach (var row in paramsCollection)
            {
                AddOrUpdate(row.Key, row.Value);
            }
        }

        public T GetAsDef<T>(string key, T def = default)
            where T : class
                => TryGetValue(key, out var value) ? value as T ?? def : def;

        public static CustomParams Clone(CustomParams cParams)
        {
            var nParams = new CustomParams();
            nParams.AddOrUpdate(cParams.ToList());

            return nParams;
        }
    }
}