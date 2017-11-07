using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Pixel.Utils;
using Pixel.Web.Helpers.Attributes;
using Pixel.Web.Helpers.Responders;

namespace Pixel.Web.Helpers
{
    public class Router
    {
        private string _controllerName, _actionName;
        private string[] _segments;
        private readonly string _applicationRootPath;
        private readonly HttpContext _context;
        private Route _currentRoute;
        private readonly Assembly _runningAssembly;
        private Controller _controller;
        private IResponder _responder;

        private readonly Dictionary<string, IResponder> _contentResponder = new Dictionary<string, IResponder>
        {
            { "",new PlainTextResponder()},
            {"application/json",new JsonResponder() },
            {"text/plain",new PlainTextResponder() },
            {"text/xml", new XmlResponder() },
            {"image",new FileResponder(null) }
        };

        public Router(Assembly runningAssembly)
        {
            _applicationRootPath = string.Empty;
            _context = HttpContext.Current;
            _runningAssembly = runningAssembly;
            SetSegments();
            if (_segments != null)
            {
                SetRoute();
            }
        }
        public Router(string applicationRootPath, Assembly runningAssembly)
        {
            _applicationRootPath = applicationRootPath;
            _context = HttpContext.Current;
            _runningAssembly = runningAssembly;
            SetSegments();
            if (_segments != null)
            {
                SetRoute();
            }
        }

        public string ControllerName => _controllerName;
        public string ActionName => _actionName.ToLower(new CultureInfo("en-US"));

        public void SetSegments()
        {
            string[] segments = HttpContext.Current.Request.Url.Segments.Select(x => x.ToLower(new CultureInfo("en-US")).Replace("/", string.Empty)).ToArray();
            if (segments.Length > 2)
            {
                if (string.IsNullOrEmpty(_applicationRootPath))
                {
                    segments = segments.Skip(2).ToArray();
                }
                else
                {
                    var indexOfRootPath =
                        Array.IndexOf(segments, _applicationRootPath.ToLower(new CultureInfo("en-US")));
                    segments = segments.Skip(indexOfRootPath + 1).ToArray();
                }

                _controllerName = segments[0];
                _actionName = segments[1];
            }
            else
            {
                segments = null;
            }
            _segments = segments;
        }

        private object[] GetParameters()
        {
            var parameterTypes = _currentRoute.Parameters;
            object[] parameters = new object[parameterTypes.Length];
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                var t = parameterTypes[i];
                if (t == typeof(HttpPostedFile))
                {
                    HttpPostedFile file = null;
                    if (_context.Request.Files.Count > 0)
                    {
                        file = _context.Request.Files[0];
                    }
                    parameters[i] = file;
                }
                else
                {
                    NameValueCollection formData = null;
                    switch (_currentRoute.RequestType)
                    {
                        case RequestType.HttpPost:
                            formData = _context.Request.Form;
                            if (!formData.AllKeys.Any())
                            {
                                formData = new NameValueCollection();
                                var content = new StreamReader(_context.Request.InputStream).ReadToEnd();
                                foreach (var s in content.Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var prms = s.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                                    formData.Add(prms[0], prms[1]);
                                }
                            }
                            break;
                        case RequestType.HttpGet:
                            formData = _context.Request.QueryString;
                            break;
                    }
                    var pVal = typeof(ObjectBinder).GetMethod("BindObject")?.MakeGenericMethod(t).Invoke(this, new object[] { formData });
                    parameters[i] = pVal;
                }
            }
            return parameters;
        }

        private void SetRoute()
        {
            var requestType = RequestType.HttpGet;
            var controller = _runningAssembly.GetTypes()
                .FirstOrDefault(c => c.BaseType == typeof(Controller) &&
                                     (c.Name == _controllerName.ToCapitalCase() ||
                                      c.Name == _controllerName.ToCapitalCase() + "Controller"));
            if (controller != null)
            {
                _controller = (Controller)Activator.CreateInstance(controller);
                var action = _controller.Actions
                    .FirstOrDefault(x => x.Name.ToLower(new CultureInfo("en-US")) == _actionName);
                if (action != null)
                {
                    if (action.IsDefined(typeof(ResponseMethodAttribute)))
                    {
                        var attributes = action.GetCustomAttribute<ResponseMethodAttribute>();
                        requestType = attributes.RequestType;
                    }
                    var currentRequestType =
                        (RequestType)Enum.Parse(typeof(RequestType), "http" + _context.Request.RequestType, true);
                    if (currentRequestType == requestType)
                    {
                        var actionParameters = action.GetParameters();
                        Type[] parameters = actionParameters.Select(t => t.ParameterType).ToArray();

                        var route = new Route
                        {
                            Controller = _controller,
                            Action = action,
                            RequestType = currentRequestType,
                            Path = _context.Request.Url.AbsolutePath,
                            Parameters = parameters
                        };
                        _currentRoute = route;
                        if (_contentResponder.ContainsKey(route.ContentType.ToLower()))
                            _responder = _contentResponder[route.ContentType];
                        else
                        {
                            throw new Exception("Unknown content type");
                        }
                    }
                    else
                    {
                        EndResponse(404);
                    }
                }
                else
                {
                    EndResponse(404);
                }
            }
            else
            {
                EndResponse(404);
            }
        }

        private void EndResponse(int statusCode)
        {
            _context.Response.StatusCode = statusCode;
            _context.Response.End();
        }

        public void ProcessRequest()
        {
            var actionParameters = GetParameters();
            var result = _currentRoute.Action.Invoke(_currentRoute.Controller, actionParameters);
            AfterActionExecuting(result);

        }
        public void AfterActionExecuting(object responseData)
        {
            _context.Response.ContentType = _currentRoute.ContentType;

            _responder.Write(_context, responseData);
            _context.Response.End();
        }
    }
}
