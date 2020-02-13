using System;

namespace NukeCore.Extensions.Http.Common.Settings
{

    public class HttpSenderOptions
    {
        private const int DEF_TIMEOUT_SEC = 60;
        private static readonly TimeSpan DefTimeout = TimeSpan.FromSeconds(DEF_TIMEOUT_SEC);

        public TimeSpan RequestTimeout { get; set; } = DefTimeout;
        public bool ValidateErrorsInResponse { get; set; } = true;
        public JsonParseSettings JsonParseSettings { get; set; } = new JsonParseSettings();
        public XmlParseSettings XmlParseSettings { get; set; } = new XmlParseSettings();
        public CustomParams CustomParams { get; set; } = new CustomParams();
    }
}