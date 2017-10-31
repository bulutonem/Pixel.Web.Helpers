using System.Web;

namespace Pixel.Web.Helpers.Responders
{
    public class XmlResponder : IResponder
    {
        public void Write(HttpContext context, object data)
        {
            context.Response.Write(Pixel.Utils.Serializers.XmlSerializerUtil.SerializeObject(data));
        }
    }
}