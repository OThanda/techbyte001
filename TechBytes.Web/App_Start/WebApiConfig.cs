using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Microsoft.Practices.Unity;
using TechBytes.Web.Dispatchers;
using TechBytes.Web.Resolvers;

namespace TechBytes.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Unity Resolver
            var unityContainer = new UnityContainer();
            config.DependencyResolver = new UnityResolver(unityContainer);
            
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{version}/{controller}/{isbn}",
                defaults: new { isbn = RouteParameter.Optional }
            );

            config.Services.Replace(typeof(IHttpControllerSelector), new VersionControllerSelector(config));
        }
    }
}
