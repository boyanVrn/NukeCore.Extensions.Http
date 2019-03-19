using System.Collections.Generic;

namespace UCS.Extensions.Http.Sender.Entities
{

    public class CustomHttpHeaders : Dictionary<string, string>
    {
        public void AddOrUpdate(string name, string value)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value)) return;
            if (ContainsKey(name)) Remove(name);
            Add(name, value);
        }
    }
}