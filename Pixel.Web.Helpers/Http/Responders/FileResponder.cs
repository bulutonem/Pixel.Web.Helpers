using System;
using System.IO;
using System.Web;

namespace Pixel.Web.Helpers.Http.Responders
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
                if (context.Response.OutputStream == null)
                    throw new ArgumentNullException(nameof(OutputStream));
            }
            else
            {
                //context.Response.WriteFile()
            }
        }
    }
}