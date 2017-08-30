using EMT.Common.Services.Base;
using EMT.Utils.Profiling;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace EMT.Web.Api.Receive
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(AutofacConfig.Register);

            if (ServiceLocator.Current.GetInstance<IProfilerService>().Enabled)
            {
                MiniProfilerSqlite.Initialize(ConfigurationManager.ConnectionStrings["mini_profiler_db"].ConnectionString);
            }
        }

        protected void Application_BeginRequest()
        {
            ServiceLocator.Current.GetInstance<IProfilerService>().Start();
        }

        protected void Application_EndRequest()
        {
            ServiceLocator.Current.GetInstance<IProfilerService>().Stop();
        }
    }
}
