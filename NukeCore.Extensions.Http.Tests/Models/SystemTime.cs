using System.Xml.Serialization;

namespace NukeCore.Extensions.Http.Tests.Models
{
    public class SystemTime
    {
        [XmlElement("DateTime")]
        public string DateTime { get; set; }
    }
}