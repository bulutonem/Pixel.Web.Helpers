using System;
using System.IO;
using System.Reflection;
using System.Web;
using Pixel.Utils;

namespace Pixel.Web.Helpers.Http.RequestBinder
{
    public class PlainTextRequestBinder : IRequestBinder
    {
        public T Bind<T>(HttpContext context)
        {
            var streamContent = new StreamReader(context.Request.InputStream).ReadToEnd();
            var textNvc = HttpUtility.ParseQueryString(streamContent);
            var pVal = typeof(ObjectBinder).GetMethod("BindObject")?.MakeGenericMethod(typeof(T)).Invoke(this, new object[] { textNvc });
            return (T)pVal;
        }

        public object Bind(HttpContext context, Type type, Assembly assembly)
        {
            var streamContent = new StreamReader(context.Request.InputStream).ReadToEnd();
            var textNvc = HttpUtility.ParseQueryString(streamContent);
            var pVal = typeof(ObjectBinder).GetMethod("BindObject")?.MakeGenericMethod(type).Invoke(this, new object[] { textNvc, assembly });
            return pVal;
        }

        public object Bind(HttpContext context, Type type)
        {
            var streamContent = new StreamReader(context.Request.InputStream).ReadToEnd();
            var textNvc = HttpUtility.ParseQueryString(streamContent);
            var pVal = typeof(ObjectBinder).GetMethod("BindObject")?.MakeGenericMethod(type).Invoke(this, new object[] { textNvc });
            return pVal;
        }
    }
}