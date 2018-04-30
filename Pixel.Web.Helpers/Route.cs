using System;
using System.Reflection;
using Pixel.Web.Helpers.Attributes;

namespace Pixel.Web.Helpers
{
    public class Route
    {
        public string Path { get; set; }
        public Controller Controller { get; set; }
        public MethodInfo Action { get; set; }
        public Type[] Parameters { get; set; }
        public RequestType RequestType { get; set; }
        public string ContentType { get; set; }
        public string ResponseContentType { get; set; } = "application/json";
        public int CacheTime { get; set; } = 0;
        public bool HasCaptcha { get; set; }
    }
}