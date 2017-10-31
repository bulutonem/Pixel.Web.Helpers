using System;
using System.IO;
using System.Web;

namespace Pixel.Web.Helpers.Responders
{
    public class FileResponder : IResponder
    {
        public FileResponder(FileStream outputStream)
        {
            OutputStream = outputStream;
        }
        public FileStream OutputStream { get; set; } = null;
        public void Write(HttpContext context, object data)
        {
            if (OutputStream == null)
            {
                throw new ArgumentNullException(nameof(OutputStream));
            }

            context.Response.Write(data);
        }
    }
}