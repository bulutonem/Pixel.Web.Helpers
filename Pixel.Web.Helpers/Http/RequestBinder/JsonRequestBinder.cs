using System;
using System.IO;
using System.Web;
using Pixel.Utils.Serializers;

namespace Pixel.Web.Helpers.Http.RequestBinder
{
    public class JsonRequestBinder : IRequestBinder
    {

        public T Bind<T>(HttpContext context)
        {
            var streamContent = new StreamReader(context.Request.InputStream).ReadToEnd();
            return Utils.Serializers.JsonSerializer.DeserializeObject<T>(streamContent);
        }

        public object Bind(HttpContext context, Type type)
        {
            var streamContent = new StreamReader(context.Request.InputStream).ReadToEnd();
            var pVal = typeof(JsonSerializer).GetMethod("DeserializeObject")?.MakeGenericMethod(type).Invoke(this, new object[] { streamContent });
            return pVal;
        }
    }
}