using Autofac;
using EMT.DAL.Sql.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.DAL.Sql
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SqlCounterRepository>().As<ICounterRepository>();
            builder.RegisterType<SqlOrgStructureRepository>().As<IOrgStructureRepository>();

            builder.RegisterType<SqlGrafanaCounterRepository>().As<IGrafanaCounterRepository>();
        }
    }
}
