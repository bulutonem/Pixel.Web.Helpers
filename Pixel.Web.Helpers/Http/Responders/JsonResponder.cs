using System.Web;

namespace Pixel.Web.Helpers.Http.Responders
{
    public class JsonResponder : IResponder
    {
        public void Write(HttpContext context, object data)
        {
            context.Response.Write(Utils.Serializers.JsonSerializer.SerializeObject(data));
        }
    }
}