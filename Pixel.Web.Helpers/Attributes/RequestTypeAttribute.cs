using System;

namespace Pixel.Web.Helpers.Attributes
{
    public sealed class RequestTypeAttribute : Attribute
    {
        public RequestTypeAttribute(ResponseMethodAttribute.RequestType type)
        {
            this.Type = type.ToString().ToUpper();
        }
        public string Type { get; set; }
    }
}