using System;
using System.Web;
using Pixel.Utils;

namespace Pixel.Web.Helpers.Http.RequestBinder
{
    public class FormRequestBinder : IRequestBinder
    {
        public T Bind<T>(HttpContext context)
        {
            var data = context.Request.Form;
            var pVal = typeof(ObjectBinder).GetMethod("BindObject")?.MakeGenericMethod(typeof(T)).Invoke(this, new object[] { data });
            return (T)pVal;
        }

        public object Bind(HttpContext context, Type type)
        {
            var data = context.Request.Form;
            var pVal = typeof(ObjectBinder).GetMethod("BindObject")?.MakeGenericMethod(type).Invoke(this, new object[] { data });
            return pVal;
        }
    }
}
