using System;

namespace Pixel.Web.Helpers.Attributes
{
    public abstract class ResponseMethodAttribute : Attribute
    {
        public RequestType RequestType { get; set; }
        /// <summary>
        /// "application/json", "image/png", "text/html" etc.
        /// </summary>
        public string ResponseType { get; set; } = "application/json";
        /// <summary>
        /// Cache time for method response. As millisecond.
        /// </summary>
        public int CacheTime { get; set; } = 0;
        public bool HasCaptcha { get; set; }

    }
}