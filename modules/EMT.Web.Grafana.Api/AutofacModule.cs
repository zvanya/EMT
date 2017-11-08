using Autofac;
using Autofac.Integration.WebApi;
using EMT.Common.Services;
using EMT.Common.Services.Base;
using EMT.DAL;
using EMT.Utils.Profiling;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace EMT.Web.Grafana.Api
{
    public class AutofacModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterApiControllers(System.Reflection.Assembly.GetExecutingAssembly());

            //TODO: исправить на string connectionString = string.Empty;
            string connectionString = ConfigurationManager.ConnectionStrings["counters_board_db"].ConnectionString;
            builder.Register(с => new DefaultDatabaseSettings(connectionString)).As<IDatabaseSettings>();

            string isProfilerEnabled = ConfigurationManager.AppSettings["profiler_enabled"];
            if (isProfilerEnabled != null && bool.Parse(isProfilerEnabled) == true)
            {
                builder.RegisterType<MiniProfilerService>().As<IProfilerService>();
            }
        }
    }
}