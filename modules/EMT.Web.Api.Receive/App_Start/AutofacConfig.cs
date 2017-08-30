using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Practices.ServiceLocation;
using Autofac.Extras.CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace EMT.Web.Api.Receive
{
    public static class AutofacConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<EMT.Common.AutofacModule>();
            builder.RegisterModule<EMT.DAL.Sql.AutofacModule>();
            builder.RegisterModule<EMT.Web.Api.Receive.AutofacModule>();
            

            builder.RegisterWebApiFilterProvider(config);

            builder.Register(c => ServiceLocator.Current).As<IServiceLocator>();

            var container = builder.Build();

            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}