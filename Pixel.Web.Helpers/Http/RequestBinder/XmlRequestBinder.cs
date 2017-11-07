using System;
using System.IO;
using System.Web;
using Pixel.Utils;
using Pixel.Utils.Serializers;

namespace Pixel.Web.Helpers.Http.RequestBinder
{
    public class XmlRequestBinder : IRequestBinder
    {
        public T Bind<T>(HttpContext context)
        {
            var streamContent = new StreamReader(context.Request.InputStream).ReadToEnd();
            return Utils.Serializers.XmlSerializerUtil.DeserializeObject<T>(streamContent);
        }

        public object Bind(HttpContext context, Type type)
        {
            var streamContent = new StreamReader(context.Request.InputStream).ReadToEnd();
            var pVal = typeof(XmlSerializerUtil).GetMethod("DeserializeObject")?.MakeGenericMethod(type).Invoke(this, new object[] { streamContent });
            return pVal;
        }
    }
}
