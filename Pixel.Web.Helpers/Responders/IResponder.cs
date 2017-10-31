using System.Web;

namespace Pixel.Web.Helpers.Responders
{
    public interface IResponder
    {
        void Write(HttpContext context, object data);
    }
}