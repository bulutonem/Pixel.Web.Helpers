using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using Pixel.Utils;
using Pixel.Web.Helpers.Attributes;
using Pixel.Web.Helpers.Http.RequestBinder;
using Pixel.Web.Helpers.Http.Responders;

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
            { "*/*",new JsonResponder()},
            {"application/json",new JsonResponder() },
            {"text/plain",new PlainTextResponder() },
            {"text/xml", new XmlResponder() },
            {"image",new FileResponder(null) }
        };
        private readonly Dictionary<string, IRequestBinder> _requestBinders = new Dictionary<string, IRequestBinder>
        {
            { "application/x-www-form-urlencoded",new FormRequestBinder()},
            { "multipart/form-data",new FormRequestBinder()},
            {"application/json",new JsonRequestBinder() },
            {"text/plain",new FormRequestBinder() },
            {"text/xml", new XmlRequestBinder() }
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
                    if (_requestBinders.ContainsKey(_currentRoute.ContentType.ToLower()))
                    {
                        var binder = _requestBinders[_currentRoute.ContentType.ToLower()];
                        parameters[i] = binder.Bind(_context, t);
                    }
                    else
                    {
                        if (Regex.IsMatch(_currentRoute.ContentType.ToLower(), $"^({string.Join("|", _requestBinders.Keys.ToArray())})"))
                        {
                            var matches = Regex.Matches(_currentRoute.ContentType.ToLower(), $"^({string.Join("|", _requestBinders.Keys.ToArray())})");
                            if (matches.Count > 0)
                            {
                                var binder = _requestBinders[matches[0].Value];
                                parameters[i] = binder.Bind(_context, t);
                            }
                        }
                        else
                        {
                            EndResponse(HttpStatusCode.BadRequest, $"Invalid content type. Content type can be one of these {string.Join("<br/>", _requestBinders.Keys.ToArray())}");
                        }
                    }
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
                            Parameters = parameters,
                            ContentType = _context.Request.ContentType
                        };
                        _currentRoute = route;
                        if (_context.Request.AcceptTypes != null)
                            foreach (var requestAcceptType in _context.Request.AcceptTypes)
                            {
                                if (_contentResponder.ContainsKey(requestAcceptType.ToLower()))
                                {
                                    _responder = _contentResponder[requestAcceptType];
                                    break;
                                }
                            }
                        if (_responder == null)
                        {
                            _responder = _contentResponder[route.DefaultResponseContentType];
                        }
                    }
                    else
                    {
                        EndResponse(HttpStatusCode.NotFound);
                    }
                }
                else
                {
                    EndResponse(HttpStatusCode.NotFound);
                }
            }
            else
            {
                EndResponse(HttpStatusCode.NotFound);
            }
        }

        private void EndResponse(HttpStatusCode statusCode)
        {
            if (!Enum.IsDefined(typeof(HttpStatusCode), statusCode))
                throw new InvalidEnumArgumentException(nameof(statusCode), (int)statusCode, typeof(HttpStatusCode));
            _context.Response.StatusCode = (int)statusCode;
            _context.Response.End();
        }

        private void EndResponse(HttpStatusCode statusCode, string message)
        {
            if (!Enum.IsDefined(typeof(HttpStatusCode), statusCode))
                throw new InvalidEnumArgumentException(nameof(statusCode), (int)statusCode, typeof(HttpStatusCode));
            _context.Response.StatusCode = (int)statusCode;
            _context.Response.StatusDescription = message;
            _context.Response.End();
        }

        public void ProcessRequest()
        {
            if (_segments == null || _controller == null || string.IsNullOrEmpty(_actionName))
            {
                _context.Response.Clear();
                EndResponse(HttpStatusCode.NotFound);
            }
            else
            {
                var actionParameters = GetParameters();
                var result = _currentRoute.Action.Invoke(_currentRoute.Controller, actionParameters);
                AfterActionExecuting(result);
            }

        }
        public void AfterActionExecuting(object responseData)
        {
            _context.Response.ContentType = _currentRoute.ContentType;

            _responder.Write(_context, responseData);
            _context.Response.End();
        }
    }
}
