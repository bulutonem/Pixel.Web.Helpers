using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Pixel.Utils.Serializers
{

    public static class XmlSerializerUtil
    {
        public static XmlDocument SerializeObject(object obj)
        {
            XmlDocument result = new XmlDocument();
            var serializingSettings = new XmlWriterSettings();
            serializingSettings.OmitXmlDeclaration = true;
            var serializer = new XmlSerializer(obj.GetType());
            XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
            xmlns.Add("", "");
            using (var memStm = new MemoryStream())
            using (var xw = XmlWriter.Create(memStm, serializingSettings))
            {
                serializer.Serialize(xw, obj, xmlns);
                memStm.Position = 0;
                result.Load(memStm);
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <param name="returnNewInstanceOnException">if value of returnNewInstanceOnException is true, method will return a new instance of T object. Else throw an exception</param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string xml, bool returnNewInstanceOnException = false)
        {
            Type type = typeof(T);
            XmlSerializer serializer = new XmlSerializer(type, string.Empty);
            TextReader reader = new StringReader(xml);
            try
            {
                var obj = (T)serializer.Deserialize(reader);
                return obj;
            }
            catch (Exception)
            {
                if (returnNewInstanceOnException)
                    return (T)Activator.CreateInstance(typeof(T));
                throw;
            }
        }

        public static XmlDocument LoadXml(string xml)
        {
            var result = new XmlDocument();
            try
            {
                result.LoadXml(xml);
            }
            catch
            {
                xml = ("<root>" + EscapeXmlValue(xml) + "</root>");
                result.LoadXml(xml);
                //result = SerializeObject(xml);
            }
            return result;
        }

        private static string EscapeXmlValue(string xmlString)
        {

            if (xmlString == null)
                throw new ArgumentNullException("xmlString");

            return xmlString;
        }
    }
}
