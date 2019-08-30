namespace UCS.Extensions.Http.Common.Models
{

    public class HttpSenderOptions
    {
        public bool ValidateErrorsInResponse { get; set; } = true;
        public JsonParseSettings JsonParseSettings { get; set; } = new JsonParseSettings();
        public XmlParseSettings XmlParseSettings { get; set; } = new XmlParseSettings();
        public CustomParams CustomParams { get; set; } = new CustomParams();
    }
}