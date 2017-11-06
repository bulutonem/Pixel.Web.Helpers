using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Pixel.Utils
{
    public static class Extensions
    {
        public static string ToCapitalCase(this string source)
        {
            return new StringUtils().ToCapitalCase(source);
        }
        public static string ToJson(this object source)
        {
            return Serializers.JsonSerializer.SerializeObject(source);
        }
        public static XmlDocument ToXml(this object source)
        {
            return Serializers.XmlSerializerUtil.SerializeObject(source);
        }
        public static T DeserializeXml<T>(this XmlDocument source) where T : class, new()
        {
            return Serializers.XmlSerializerUtil.DeserializeObject<T>(source.InnerXml);
        }
        public static T DeserializeXml<T>(this string source) where T : class, new()
        {
            return Serializers.XmlSerializerUtil.DeserializeObject<T>(source);
        }
        public static T DeserializeJson<T>(this string source) where T : class, new()
        {
            return Serializers.JsonSerializer.DeserializeObject<T>(source);
        }
    }
}
