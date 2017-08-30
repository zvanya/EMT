using EMT.Common.Services.Base;
using EMT.DAL;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMT.Web.Api.Models;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using EMT.Web.Api.Common.Controllers;

namespace EMT.Web.Api.Controllers
{
    [RoutePrefix("api/counters")]
    public class CountersController : BaseApiController
    {
        #region Fields

        private readonly ICounterRepository _counterRepository;
        private readonly IDateTimeService _dateTimeService;
        private readonly ICsvService _csvService;

        #endregion

        #region Constructors

        public CountersController(IServiceLocator serviceLocator): base(serviceLocator)
        {
            _counterRepository = this.ServiceLocator.GetInstance<ICounterRepository>();
            _dateTimeService = this.ServiceLocator.GetInstance<IDateTimeService>();
            _csvService = this.ServiceLocator.GetInstance<ICsvService>();
        }

        #endregion

        #region Web Actions

        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            var counters = _counterRepository.GetCounters();

            var countersJson = counters
                .Select(c => new
                {
                    id = c.Id,
                    tag = c.Tag?.Trim(),
                    lineId = c.LineId,
                    name = c.Name?.Trim(),
                    color = c.Color,
                    iso = c.ISO?.Trim(),
                    min = c.Min,
                    max = c.Max
                })
                .ToList();

            return Ok(countersJson);
        }        

        [Route("{counterId}/values")]
        [HttpGet]
        public IHttpActionResult GetValues(int counterId, long timeFrom, long timeTo)
        {
            var data = GetCounterValues(counterId, timeFrom, timeTo);
            return Ok(data);
        }

        [Route("{counterId}/values/csv")]
        [HttpGet]
        public HttpResponseMessage GetValuesCsv(int counterId, long timeFrom, long timeTo)
        {
            var data = GetCounterValues(counterId, timeFrom, timeTo);
            return GetCsvResponse(data, counterId);
        }

        [Route("{counterId}/values")]
        [HttpGet]
        public IHttpActionResult GetValues(int counterId, long after)
        {
            var data = GetCounterValues(counterId, after);
            return Ok(data);
        }

        [Route("{counterId}/values/csv")]
        [HttpGet]
        public HttpResponseMessage GetValuesCsv(int counterId, long after)
        {
            var data = GetCounterValues(counterId, after);
            return GetCsvResponse(data, counterId);
        }

        #endregion

        #region Helpers

        private List<CounterValueModel> GetCounterValues(int counterId, long timeFrom, long timeTo)
        {
            var counterValues = _counterRepository.GetValues(
                counterId,
                _dateTimeService.UnixTimeToDateTime(timeFrom),
                _dateTimeService.UnixTimeToDateTime(timeTo));

            var models = counterValues
                .Select(c => new CounterValueModel()
                {
                    Time = _dateTimeService.DateTimeToUnixTime(c.dt),
                    Value = c.value
                })
                .ToList();
            return models;
        }

        private List<CounterValueModel> GetCounterValues(int counterId, long after)
        {
            var counterValues = _counterRepository.GetValues(
                counterId,
                _dateTimeService.UnixTimeToDateTime(after));

            var models = counterValues
                .Select(c => new CounterValueModel()
                {
                    Time = _dateTimeService.DateTimeToUnixTime(c.dt),
                    Value = c.value
                })
                .ToList();
            return models;
        }

        private HttpResponseMessage GetCsvResponse(List<CounterValueModel> data, int counterId)
        {
            var csv = _csvService.ToCSV(data, true);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StringContent(csv);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = string.Format("counter-{0}-values.csv", counterId) };
            return result;
        }

        #endregion

    }
}