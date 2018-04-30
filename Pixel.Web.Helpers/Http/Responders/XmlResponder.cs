using System.Web;
using System.Xml;

namespace Pixel.Web.Helpers.Http.Responders
{
    public class XmlResponder : IResponder
    {
        public void Write(HttpContext context, object data)
        {
            XmlDocument result;
            if (data.GetType() == typeof(XmlDocument))
            {
                result = (XmlDocument)data;
            }
            else
            {
                result = Pixel.Utils.Serializers.XmlSerializerUtil.SerializeObject(data);
            }
            context.Response.Write(result.InnerXml);
        }
    }
}