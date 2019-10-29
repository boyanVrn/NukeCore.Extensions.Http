namespace NukeCore.Extensions.Http.Common.Models
{
    public class XmlParseSettings
    {
        public XmlSerializeSettings Serialize { get; set; }
        public XmlSerializeSettings Deserialize { get; set; }

        public XmlParseSettings()
        {
            Serialize = new XmlSerializeSettings();
            Deserialize = new XmlSerializeSettings();
        }
    }
}
