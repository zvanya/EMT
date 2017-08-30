using EMT.Web.Api.Common.Controllers;
using EMT.Web.Tools.Generator.Code.Jobs;
using EMT.Web.Tools.Generator.Code.Services;
using EMT.Web.Tools.Generator.Models;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace EMT.Web.Tools.Generator.Controllers
{
    [RoutePrefix("api/generation/counters/jobs")]
    public class CounterJobsController : BaseApiController
    {
        private readonly ICounterService _counterService;
        private readonly IOrgStructureService _orgStructureService;
        private readonly CounterJobManager _counterJobManager;

        public CounterJobsController(IServiceLocator serviceLocator) : base(serviceLocator)
        {
            _counterService = this.ServiceLocator.GetInstance<ICounterService>();
            _counterJobManager = this.ServiceLocator.GetInstance<CounterJobManager>();
            _orgStructureService = this.ServiceLocator.GetInstance<IOrgStructureService>();
        }

        [HttpGet]
        [Route("")]
        public IEnumerable<CounterJobModel> GetJobs()
        {
            var countersTask = Task.Run(() => _counterService.GetCounters());
            var orgstructureTask = Task.Run(() => _orgStructureService.GetUnits());

            Task.WaitAll(countersTask, orgstructureTask);

            var countersResponse = countersTask.Result;
            var orgstructureResponse = orgstructureTask.Result;
            var orgstrucruteUnits = orgstructureResponse.Data;

            var counterJobs = countersResponse.Data
                .Select(r => new CounterJobModel()
                {
                    Id = r.Id,
                    LineFullName = GetLineFullName(r.LineId.Value, orgstrucruteUnits),
                    Tag = r.Tag,
                    LineId = r.LineId,
                    Name = r.Name,
                    Color = r.Color,
                    ISO = r.ISO,
                    Min = r.Min,
                    Max = r.Max,
                    TimeInterval = 1000,
                    InProgress = _counterJobManager.IsStarted(r.Id)
                })
                .OrderBy(r => r.LineFullName)
                .ThenBy(r => r.Name);

            return counterJobs;
        }

        [HttpPost]
        [Route("start")]
        public IHttpActionResult StartJob([FromBody]CounterJobModel counterJob)
        {
            _counterJobManager.Start(counterJob.Id, counterJob.TimeInterval, null);
            return Ok(counterJob);
        }

        [HttpPost]
        [Route("stop")]
        public IHttpActionResult StopJob([FromBody]CounterJobModel counterJob)
        {
            _counterJobManager.Stop(counterJob.Id);
            return Ok(counterJob);
        }

        private string GetLineFullName(int lineId, IEnumerable<OrgStructureUnitModel> orgStructureUnits)
        {
            var line = orgStructureUnits.Single(r => r.Id == lineId && r.IsLine == true);
            var names = line.Path
                .Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(r => int.Parse(r))
                .Select(r => orgStructureUnits.Where(o => o.Id == r).Select(o => o.Name).Single())
                .ToArray();

            return string.Join(" / ", names);
        }
    }

}