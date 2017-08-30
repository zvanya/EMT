using EMT.Common.Services.Base;
using EMT.DAL;
using EMT.Entities;
using EMT.Web.Api.Common.Controllers;
using EMT.Web.Api.Receive.Models;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Logging;
using System.Reflection;

namespace EMT.Web.Api.Controllers
{
    [RoutePrefix("api/counters")]
    public class CountersController: BaseApiController
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ICounterRepository _counterRepository;
        private readonly IDateTimeService _dateTimeService;
        private readonly ICsvService _csvService;

        public CountersController(IServiceLocator serviceLocator):base(serviceLocator)
        {
            _counterRepository = this.ServiceLocator.GetInstance<ICounterRepository>();
            _dateTimeService = this.ServiceLocator.GetInstance<IDateTimeService>();
            _csvService = this.ServiceLocator.GetInstance<ICsvService>();
        }

        [Route("values")]
        [HttpPost]
        public void Post([FromBody]CounterValueModel[] counterValues)
        {
            var items = counterValues
                .Select(r => new CounterValue()
                {
                    id_counter = r.CounterId,
                    dt = _dateTimeService.UnixTimeToDateTime(r.Time),
                    value = r.Value
                });

            _counterRepository.Insert(items);
        }

        [Route("values/csv")]
        [HttpPost]
        public void PostCsv([FromBody]string counterValuesCsv)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("[PostCsv] counterValuesCsv = {0}", counterValuesCsv);
            }

            var models = _csvService.FromCSV<CounterValueModel>(counterValuesCsv, true);
            var items = models
                .Select(r => new CounterValue()
                {
                    id_counter = r.CounterId,
                    dt = _dateTimeService.UnixTimeToDateTime(r.Time),
                    value = r.Value
                });

            _counterRepository.Insert(items);
        }
    }
}
