using System;

namespace Pixel.Web.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPostAttribute : ResponseMethodAttribute
    {
        public HttpPostAttribute()
        {
            RequestType = RequestType.HttpPost;
        }
    }
}