using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.ServiceLocation;
using EMT.Web.Tools.Generator.Code.Services;
using EMT.Common.Services.Base;
using EMT.Web.Tools.Generator.Models;
using RestSharp;

namespace EMT.Web.Tools.Generator.Code.Jobs
{
    public class CounterJob : BaseJob<CounterJobArgs>
    {
        private readonly IServiceLocator _serviceLocator;

        private readonly int _counterId;
        private readonly ICounterService _counterService;
        private readonly IDateTimeService _dateTimeService;
        private readonly Random _random;

        public CounterJob(int counterId, IServiceLocator serviceLocator) : base(serviceLocator)
        {
            _serviceLocator = serviceLocator;
            _counterId = counterId;
            _counterService = _serviceLocator.GetInstance<ICounterService>();
            _dateTimeService = _serviceLocator.GetInstance<IDateTimeService>();
            _random = new Random();
        }

        public int CounterId => _counterId;

        protected override void Perform(CounterJobArgs args)
        {
            var value = _random.NextDouble();
            var time = _dateTimeService.Now();
            CounterValueModel item = new CounterValueModel()
            {
                CounterId = _counterId,
                Time = time,
                Value = value
            };
            List<CounterValueModel> items = new List<CounterValueModel>();
            items.Add(item);
            IRestResponse response = _counterService.Insert(items);
            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }
        }
    }
}