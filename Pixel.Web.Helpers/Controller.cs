using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Web;

namespace Pixel.Web.Helpers
{
    public abstract class Controller : IDisposable
    {
        public string ApplicationRootPath { get; }

        //private Router _router;
        private readonly HttpContext _context;

        public HttpContext Context => _context;
        public List<MethodInfo> Actions { get; set; }

        protected Controller()
        {
            _context = HttpContext.Current;
            //_router = new Router();

            Actions = GetType().GetMethods().ToList();
        }

        protected Controller(string applicationRootPath)
        {
            ApplicationRootPath = applicationRootPath;
            _context = HttpContext.Current;
            //_router = new Router(applicationRootPath);
            Actions = GetType().GetMethods().ToList();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        ~Controller()
        {
            Dispose();
        }
    }
}
