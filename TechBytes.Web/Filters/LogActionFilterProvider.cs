using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Microsoft.Practices.Unity;

namespace TechBytes.Web.Filters
{
    public class LogActionFilterProvider : ActionDescriptorFilterProvider, IFilterProvider
    {
        private readonly IUnityContainer _container;

        public LogActionFilterProvider(IUnityContainer container)
        {
            _container = container;
        }
 
        public new IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration, HttpActionDescriptor actionDescriptor)
        {
            var filters = base.GetFilters(configuration, actionDescriptor);
            var filterInfoList = new List<FilterInfo>();
 
            foreach (var filter in filters)
            {
                _container.BuildUp(filter.Instance.GetType(), filter.Instance);
            }
 
            return filters;
        }
    }
}