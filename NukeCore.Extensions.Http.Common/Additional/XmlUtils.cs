using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace UCS.Extensions.Http.Common.Additional
{
    public static class XmlUtils
    {
        public static XDocument CastObjToXDocument<T>(T obj)
        {
            var doc = new XDocument();

            if (obj == null) return doc;

            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var writer = doc.CreateWriter())
            {
                xmlSerializer.Serialize(writer, obj);
            }

            return doc;
        }

        public static T CastXDocumentToObj<T>(XDocument doc)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));

            if (doc.Root == null) return default(T);

            using (var reader = doc.Root.CreateReader())
            {
                return (T)xmlSerializer.Deserialize(reader);
            }
        }

        public static void RemoveEmptyElementsFrom(XDocument doc)
            => doc.Descendants().Where(e => string.IsNullOrEmpty(e.Value)).Remove();
    }
}
