using System;

namespace Pixel.Web.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpGetAttribute : ResponseMethodAttribute
    {
        public HttpGetAttribute()
        {
            RequestType = RequestType.HttpGet;
        }
    }
}
