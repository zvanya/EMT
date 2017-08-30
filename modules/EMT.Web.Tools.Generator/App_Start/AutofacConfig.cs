using Autofac;
using Microsoft.Practices.ServiceLocation;
using Autofac.Extras.CommonServiceLocator;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;

namespace EMT.Web.Tools.Generator
{
    public static class AutofacConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<EMT.Common.AutofacModule>();
            builder.RegisterModule<EMT.Web.Tools.Generator.AutofacModule>();

            builder.RegisterFilterProvider();
            builder.RegisterWebApiFilterProvider(config);

            builder.Register(c => ServiceLocator.Current).As<IServiceLocator>();

            var container = builder.Build();

            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}