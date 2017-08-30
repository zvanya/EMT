using Autofac;
using EMT.Common.Services;
using EMT.Common.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.Common
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DateTimeService>().As<IDateTimeService>();
            builder.RegisterType<CsvService>().As<ICsvService>();
            builder.RegisterType<DefaultProfilerService>().As<IProfilerService>();
        }
    }
}
