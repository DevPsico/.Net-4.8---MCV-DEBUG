using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http; // <-- Ensure this using directive is present
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebApplication1.App_Start;

namespace WebApplication1
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // Configurar Web API
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(DependencyInjectionConfig.Register);
            GlobalConfiguration.Configure(AutoMapperConfig.Register);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
