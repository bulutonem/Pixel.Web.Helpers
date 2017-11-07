using System.Web;

namespace Pixel.Web.Helpers.Http.Responders
{
    public interface IResponder
    {
        void Write(HttpContext context, object data);
    }
}