using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using EMT.Web.Tools.Generator.Code;
using EMT.Web.Tools.Generator.Code.Jobs;
using EMT.Web.Tools.Generator.Code.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace EMT.Web.Tools.Generator
{
    public class AutofacModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterControllers(System.Reflection.Assembly.GetExecutingAssembly());
            builder.RegisterApiControllers(System.Reflection.Assembly.GetExecutingAssembly());

            builder.RegisterType<CounterService>().As<ICounterService>();
            builder.RegisterType<OrgStructureService>().As<IOrgStructureService>();
            builder.RegisterType<DataServicesSettings>().As<IDataServicesSettings>();
            builder.RegisterType<CounterJobManager>().SingleInstance();

        }
    }
}