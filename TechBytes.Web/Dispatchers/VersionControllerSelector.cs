using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace TechBytes.Web.Dispatchers
{
    public class VersionControllerSelector : IHttpControllerSelector
    {
        private readonly HttpConfiguration _configuration;
        private readonly Lazy<Dictionary<string, HttpControllerDescriptor>> _controllers;

        public VersionControllerSelector(HttpConfiguration config)
        {
            _configuration = config;
            _controllers = new Lazy<Dictionary<string, HttpControllerDescriptor>>(InitializeControllers);
        }
        
        /// <summary>
        /// Selects a <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> for the given <see cref="T:System.Net.Http.HttpRequestMessage" />.
        /// </summary>
        /// <remarks>
        /// This method is used to reroute the request url to the correct version.
        /// </remarks>
        /// <param name="request">The HTTP request message.</param>
        /// <returns>
        /// An <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> instance.
        /// </returns>
        /// <exception cref="System.Web.Http.HttpResponseException">
        /// </exception>
        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var routeData = request.GetRouteData();

            if (null == routeData)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var controllerName = GetControllerName(routeData);

            if (null == controllerName)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var namespaceName = GetVersion(routeData);

            if (null == namespaceName)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var controllerKey = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", namespaceName, controllerName);

            HttpControllerDescriptor controllerDescriptor;

            if (_controllers.Value.TryGetValue(controllerKey, out controllerDescriptor))
                return controllerDescriptor;

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            return _controllers.Value;
        }

        /// <summary>
        /// Creates a dictionary list with keys based on namespace
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, HttpControllerDescriptor> InitializeControllers()
        {
            var dictionary = new Dictionary<string, HttpControllerDescriptor>(StringComparer.OrdinalIgnoreCase);

            var assembliesResolver = _configuration.Services.GetAssembliesResolver();
            var controllersResolver = _configuration.Services.GetHttpControllerTypeResolver();
            var controllerTypes = controllersResolver.GetControllerTypes(assembliesResolver);

            foreach (var controllerType in controllerTypes)
            {
                var segments = controllerType.Namespace.Split(Type.Delimiter);
                var controllerName = controllerType.Name.Remove(controllerType.Name.Length - DefaultHttpControllerSelector.ControllerSuffix.Length);
                var controllerKey = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", segments[segments.Length - 1], controllerName);

                if (!dictionary.Keys.Contains(controllerKey))
                    dictionary[controllerKey] = new HttpControllerDescriptor(_configuration, controllerType.Name, controllerType);
            }

            return dictionary;
        }

        /// <summary>
        /// Gets the route variable by matching the parameter, name, with routeData values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="routeData">The route data.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private T GetRouteVariable<T>(IHttpRouteData routeData, string name)
        {
            object result;
            if (routeData.Values.TryGetValue(name, out result))
            {
                return (T)result;
            }
            return default(T);
        }

        /// <summary>
        /// Gets the name of the controller from RouteData using the method GetSubRoutes.
        /// </summary>
        /// <param name="routeData">The route data.</param>
        /// <returns></returns>
        private string GetControllerName(IHttpRouteData routeData)
        {
            var subroutes = routeData.GetSubRoutes();

            if (null == subroutes)
                return GetRouteVariable<string>(routeData, "controller");

            var subroute = subroutes.FirstOrDefault();

            if (null == subroute)
                return null;

            var dataTokenValue = subroute.Route.DataTokens.First().Value;

            if (null == dataTokenValue)
                return null;

            return ((HttpActionDescriptor[])dataTokenValue).First().ControllerDescriptor.ControllerName.Replace("controller", string.Empty);
        }

        /// <summary>
        /// Gets the version number from RouteData using the method GetSubRoutes.
        /// </summary>
        /// <param name="routeData">The route data.</param>
        /// <returns></returns>
        private string GetVersion(IHttpRouteData routeData)
        {
            var subroutes = routeData.GetSubRoutes();

            if (null == subroutes)
                return GetRouteVariable<string>(routeData, "version");

            var subRouteData = subroutes.FirstOrDefault();

            if (null == subRouteData)
                return null;

            return GetRouteVariable<string>(subRouteData, "version");
        }
    }
}