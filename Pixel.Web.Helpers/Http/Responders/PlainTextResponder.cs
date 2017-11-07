using System.Web;

namespace Pixel.Web.Helpers.Http.Responders
{
    public class PlainTextResponder : IResponder
    {
        public void Write(HttpContext context, object data)
        {
            context.Response.Write(data);
        }
    }
}
