using System;
using System.Web;

namespace Pixel.Web.Helpers.Http.RequestBinder
{
    public interface IRequestBinder
    {
        T Bind<T>(HttpContext context);
        object Bind(HttpContext context, Type type);
    }

    //public interface IRequestBinder<in TSource> : IRequestBinder
    //{
    //    T Bind<T>(TSource data);
    //}
}