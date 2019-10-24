using Newtonsoft.Json;

namespace NukeCore.Extensions.Http.Common.Models
{
    public class JsonParseSettings
    {

        public JsonParseSettings(JsonSerializerSettings ss, JsonSerializerSettings ds)
        {
            Serializing = ss ?? new JsonSerializerSettings();
            Deserializing = ds ?? new JsonSerializerSettings();
        }

        public JsonParseSettings()
        {
            Serializing = new JsonSerializerSettings();
            Deserializing = new JsonSerializerSettings();
        }

        public JsonSerializerSettings Serializing { get; set; }
        public JsonSerializerSettings Deserializing { get; set; }
    }
}
