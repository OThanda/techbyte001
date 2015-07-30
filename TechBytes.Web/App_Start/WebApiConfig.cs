using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Filters;
using Microsoft.Practices.Unity;
using TechBytes.Web.Dispatchers;
using TechBytes.Web.Filters;
using TechBytes.Web.Loggers;
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

            unityContainer.RegisterType<IRecorder, Recorder>();
            unityContainer.RegisterType<LogActionFilter>();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{version}/{controller}/{isbn}",
                defaults: new { isbn = RouteParameter.Optional }
            );

            config.Services.Replace(typeof(IHttpControllerSelector), new VersionControllerSelector(config));

            // Add Unity filters provider
            config.Services.Replace(typeof(System.Web.Http.Filters.IFilterProvider), new LogActionFilterProvider(unityContainer));

            //var providers = config.Services.GetFilterProviders().ToList();
            //config.Services.Add(typeof(System.Web.Http.Filters.IFilterProvider), new LogActionFilterProvider(unityContainer));
            //var defaultprovider = providers.First(p => p is ActionDescriptorFilterProvider);
            //config.Services.Remove(typeof(System.Web.Http.Filters.IFilterProvider), defaultprovider);
        }
    }
}
