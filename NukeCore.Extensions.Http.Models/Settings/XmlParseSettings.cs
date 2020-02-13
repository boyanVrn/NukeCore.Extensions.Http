namespace NukeCore.Extensions.Http.Models.Settings
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
