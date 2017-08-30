using EMT.Common.Services.Base;
using EMT.Web.Tools.Generator.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMT.Web.Tools.Generator.Code.Services
{
    public class CounterService: ICounterService
    {
        #region Fields

        private readonly IDataServicesSettings _dataServicesSettings;
        private readonly IDateTimeService _dataTimeService;

        #endregion

        #region Constructors

        public CounterService(IDataServicesSettings dataServicesSettings, IDateTimeService dataTimeService)
        {
            _dataServicesSettings = dataServicesSettings;
            _dataTimeService = dataTimeService;
        }

        #endregion

        #region ICounterService

        public IRestResponse<List<CounterModel>> GetCounters()
        {
            var client = new RestClient(_dataServicesSettings.CountersEntpoint);

            var request = new RestRequest("", Method.GET);

            var response = client.Execute<List<CounterModel>>(request);            

            return response;
        }
        public IRestResponse Insert(IEnumerable<CounterValueModel> values)
        {
            var client = new RestClient(_dataServicesSettings.CounterValuesReceiveEntpoint);

            var items = values.Select(r => new
            {
                c = r.CounterId,
                v = r.Value,
                t = _dataTimeService.DateTimeToUnixTime(r.Time)
            });

            var request = new RestRequest("", Method.POST);
            request.AddJsonBody(items);

            var response = client.Execute(request);

            return response;
        }

        #endregion

    }
}