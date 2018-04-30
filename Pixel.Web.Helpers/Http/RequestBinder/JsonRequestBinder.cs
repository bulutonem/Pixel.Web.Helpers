using System;
using System.IO;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;

namespace Pixel.Web.Helpers.Http.RequestBinder
{
    public class JsonRequestBinder : IRequestBinder
    {

        public T Bind<T>(HttpContext context)
        {
            var streamContent = new StreamReader(context.Request.InputStream).ReadToEnd();
            return Utils.Serializers.JsonSerializer.DeserializeObject<T>(streamContent);
        }

        public object Bind(HttpContext context, Type type, Assembly assembly)
        {
            var streamContent = new StreamReader(context.Request.InputStream).ReadToEnd();
            var obj = JsonConvert.DeserializeObject(streamContent, type);
            return obj;
        }

        public object Bind(HttpContext context, Type type)
        {
            var streamContent = new StreamReader(context.Request.InputStream).ReadToEnd();
            var obj = JsonConvert.DeserializeObject(streamContent, type);
            return obj;
        }
    }
}