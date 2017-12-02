using EMT.Common.Services.Base;
using EMT.DAL;
using EMT.Entities;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using EMT.Web.Api.Common.Controllers;
using EMT.Web.Grafana.Api.Models;
using Newtonsoft.Json.Linq;
using System.Web.Http.Results;
using Common.Logging;
using System.Reflection;

namespace EMT.Web.Grafana.Api.Controllers
{
    [RoutePrefix("")]
    public class CountersController : BaseApiController
    {
        #region Fields

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly DateTime unixTimeStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified);

       // private readonly IGrafanaCounterRepository _grafanaCounterRepository;
        private IGrafanaCounterRepository _grafanaCounterRepository;
        private readonly IDateTimeService _dateTimeService;
        private readonly ICsvService _csvService;

        private string connectionStringName = "EMT2"; //!!!!!!!!!!!!!!!!!!!!!
        private int connectionStringNameId = 0;

        const string lineState = "__productionProcess";
        const string lineMode = "__operationMode";
        const string brand = "__brandName";
        
        #endregion

        #region Constructors

        public CountersController(IServiceLocator serviceLocator) : base(serviceLocator)
        {
            _grafanaCounterRepository = this.ServiceLocator.GetInstance<IGrafanaCounterRepository>();
            _dateTimeService = this.ServiceLocator.GetInstance<IDateTimeService>();
            _csvService = this.ServiceLocator.GetInstance<ICsvService>();
        }

        #endregion

        #region Web Actions

        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok();
        }

        [Route("search")]
        [HttpPost]
        public IHttpActionResult MetricSearchQuery([FromBody]SearchQueryObjectModel query)
        {
            bool lineInfo = @query.lineinfo;

            connectionStringName = @query.user.orgName;
            _grafanaCounterRepository.ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            string searchQuery = @query.target;
            string[] s = searchQuery.Split('.');

            if (lineInfo)
            {
                if (s.Count() == 3)
                {
                    if (s[2] == "*")
                    {
                        var result = new List<dynamic>();
                        result.Add(
                            new
                            {
                                Id = 10000,
                                text = lineState,
                                value = s[0] + "." + s[1] + "." + lineState,
                                expandable = 0
                            });
                        result.Add(
                            new
                            {
                                Id = 10001,
                                text = brand,
                                value = s[0] + "." + s[1] + "." + brand,
                                expandable = 0
                            });
                        result.Add(
                            new
                            {
                                Id = 10002,
                                text = lineMode,
                                value = s[0] + "." + s[1] + "." + lineMode,
                                expandable = 0
                            });

                        return Ok(result);
                    }
                    else if(s[2] == lineState || s[2] == lineMode || s[2] == brand)
                    {
                        var result = new List<dynamic>();

                        int num = 0;
                        if (s[2] == lineState) num = 10000;
                        else if (s[2] == lineMode) num = 10001;
                        else if (s[2] == brand) num = 10002;

                        result.Add(
                            new
                            {
                                Id = num,
                                text = s[2],
                                value = s[0] + "." + s[1] + "." + s[2],
                                expandable = 0
                            });
                        return Ok(result);
                    }
                    else
                    {
                        var result = new List<dynamic>();
                        return Ok(result);
                    }
                }
                else
                {
                    var result = new List<dynamic>();
                    return Ok(result);
                }
            }
            else
            {
                switch (s.Count())
                {
                    case 1: //GetWerks
                        {
                            var result = new List<Entities.Werk>();

                            //result = _grafanaCounterRepository.GetWerks() as List<Entities.Werk>;

                            if (s[0] == "*")
                            {
                                result.Clear();
                                result = _grafanaCounterRepository.GetWerks() as List<Entities.Werk>;
                            }
                            else
                            {
                                result.Clear();
                                //result.Add(new Entities.Werk { id = _grafanaCounterRepository.GetWerkId(s[0]).id, name = s[0] });
                                result = new List<Entities.Werk>() { new Entities.Werk { id = _grafanaCounterRepository.GetWerkId(s[0]).id, name = s[0] } };
                            }

                            var resultJson = result
                                .Select(r => new
                                {
                                    Id = r.id,
                                    text = r.name,
                                    value = r.name,
                                    expandable = 1
                                })
                                .ToList();

                            return Ok(resultJson);
                        }
                    case 2: //GetLines
                        {
                            var result = new List<Entities.Line>();
                            if (s[1] == "*")
                            {
                                result = _grafanaCounterRepository.GetLines(s[0]) as List<Entities.Line>;
                            }
                            else
                            {
                                result = new List<Entities.Line>() { new Entities.Line { id = _grafanaCounterRepository.GetLineId(s[0], s[1]).id, name = s[1] } };
                            }

                            var resultJson = result
                                .Select(r => new
                                {
                                    Id = r.id,
                                    text = r.name,
                                    value = s[0] + "." + r.name,
                                    expandable = 1
                                })
                                .ToList();

                            return Ok(resultJson);
                        }
                    case 3: //GetCounters
                        {
                            var result = new List<Entities.Counter>();
                            if (s[2] == "*")
                            {
                                result = _grafanaCounterRepository.GetCounters(s[0], s[1]) as List<Entities.Counter>;
                            }
                            else
                            {
                                result = new List<Entities.Counter>() { new Entities.Counter { Id = _grafanaCounterRepository.GetCounterId(s[0], s[1], s[2]).Id, Name = s[2] } };
                            }
                            var resultJson = result
                                .Select(r => new
                                {
                                    Id = r.Id,
                                    text = r.Name,
                                    value = s[0] + "." + s[1] + "." + r.Name,
                                    expandable = 0
                                })
                                .ToList();

                            return Ok(resultJson);
                        }
                    default:
                        {
                            var result = new List<dynamic>();
                            return Ok(result);
                        }
                }
            }
        }

        [Route("query")]
        [HttpPost]
        public IHttpActionResult Query([FromBody]QueryObjectModel query)
        {
            bool lineInfo = false;
            string[] searchQuery = { string.Empty };

            connectionStringName = @query.user.orgName;
            _grafanaCounterRepository.ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            //Точность(в процентах) прореживания исходных данных
            double eps_start = double.Parse(ConfigurationManager.AppSettings.GetValues("eps_start")[0]);
            double eps_end = double.Parse(ConfigurationManager.AppSettings.GetValues("eps_end")[0]);
            int eps_step_count = int.Parse(ConfigurationManager.AppSettings.GetValues("eps_step_count")[0]);
            
            List<DataPointsModel> dataPoints = new List<DataPointsModel>();
            List<DataPointTXTModel> dataPointsTXT = new List<DataPointTXTModel>();

            string SQL = string.Empty;
            long from;
            long to;
            int maxDataPoints;
            List<string> dataName = new List<string>();
            List<int> counterId = new List<int>();

            from = _dateTimeService.DateTimeToUnixTime(DateTime.Parse(query.range.from));
            to = _dateTimeService.DateTimeToUnixTime(DateTime.Parse(query.range.to));
            //Максимальное количество точек на графике
            maxDataPoints = query.maxDataPoints;

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("[PostQuery] from = {0}", from);
                log.DebugFormat("[PostQuery] to = {0}", to);
            }

            foreach (var t in query.targets)
            {
                List<string> targets = new List<string>();
                targets = t.target.Replace("{", "").Replace("}", "").Split(',').Select(s => s.Trim()).ToList();

                foreach (var target in targets)
                {
                    dataName.Add(target);
                }

                var counter = new Entities.Counter();
                try
                {
                    foreach (var target in targets)
                    {
                        searchQuery = target.Split('.');

                        if (searchQuery[2] != lineState && searchQuery[2] != lineMode && searchQuery[2] != brand)
                        {
                            //Вычисление id counter'a по t.target = "Харьков[KHR].PET 3.Проводимость"
                            counter = _grafanaCounterRepository.GetCounterId(searchQuery[0], searchQuery[1], searchQuery[2]) as Entities.Counter;

                            counterId.Add(counter.Id);
                        }
                        else
                        {
                            lineInfo = true;
                        }
                    }
                }
                catch (Exception) { }

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("[PostQuery] counterId = {0}", counter.Id);
                }
            }

            if (!lineInfo)
            {
                int i = 0;
                foreach (var c in counterId)
                {
                    var data = GetCounterValues(c, from, to);

                    DataPointsModel d = new DataPointsModel(); // { target = dataName[0], datapoints = new List<List<int>>() { new List<int>() { 1, 2 }, new List<int>() { 2, 3 } } };
                    d.target = dataName[i].Split('.')[2];
                    d.datapoints = new List<List<double>>();

                    foreach (var item in data)
                    {
                        d.datapoints.Add(new List<double>() { item.Value, item.Time });
                    }


                    if (connectionStringName == "UnileverRU001")
                    {
                        if (c != 55)
                        {
                            dataPoints.Add(Filtration(d, maxDataPoints, eps_start, eps_end, eps_step_count));
                            //dataPoints.Add(d);
                        }
                        else if (c == 55)
                        {
                            data.Clear();
                            data = GetCounterValues(8, from, to);

                            dataPoints.Add(
                            new DataPointsModel()
                            {
                                target = d.target,
                                datapoints = new List<List<double>>()
                                {
                                    new List<double>()
                                    {
                                        data.Last().Value - data.First().Value,
                                        data.Last().Time
                                    }
                                }
                            });
                        }
                    }
                    else
                    {
                        dataPoints.Add(Filtration(d, maxDataPoints, eps_start, eps_end, eps_step_count));
                        //dataPoints.Add(d);
                    }

                    i++;
                }

                return Json(dataPoints);
            }
            else
            {
                List<string> line = new List<string>() { "pointNumber", "pointName", "commentText" };

                foreach (string l in line)
                {
                    var data = GetLineValues(searchQuery[2], searchQuery[0], searchQuery[1], from, to);

                    DataPointTXTModel d_txt = new DataPointTXTModel();
                    d_txt.target = l;
                    d_txt.datapoints = new List<List<string>>();

                    switch (l)
                    {
                        case "pointNumber":
                            {
                                foreach (var item in data)
                                {
                                    d_txt.datapoints.Add(new List<string>() { item.Value.ToString(), item.Time.ToString() });
                                }

                                break;
                            }
                        case "pointName":
                            {
                                foreach (var item in data)
                                {
                                    d_txt.datapoints.Add(new List<string>() { item.Name, item.Time.ToString() });
                                }

                                break;
                            }
                        case "commentText":
                            {
                                foreach (var item in data)
                                {
                                    d_txt.datapoints.Add(new List<string>() { item.Comment, item.Time.ToString() });
                                }

                                break;
                            }
                    }

                    dataPointsTXT.Add(d_txt);
                }

                return Json(dataPointsTXT);
            }
        }

        [Route("query/line-status")]
        [HttpPost]
        public IHttpActionResult QueryLineStatus([FromBody]QueryObjectModel query)
        {
            connectionStringName = @query.user.orgName;
            _grafanaCounterRepository.ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            List<DataPointTXTModel> dataPointsTXT = new List<DataPointTXTModel>();

            string SQL = string.Empty;
            long from;
            long to;
            int maxDataPoints;
            List<string> dataName = new List<string>();

            from = _dateTimeService.DateTimeToUnixTime(DateTime.Parse(query.range.from));
            to = _dateTimeService.DateTimeToUnixTime(DateTime.Parse(query.range.to));
            //Максимальное количество точек на графике
            maxDataPoints = query.maxDataPoints;

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("[PostQuery] from = {0}", from);
                log.DebugFormat("[PostQuery] to = {0}", to);
            }

            foreach (var t in query.targets)
            {
                List<string> targets = new List<string>();
                targets = t.target.Replace("{", "").Replace("}", "").Split('.').Select(s => s.Trim()).ToList();

                var data = GetLineValues(targets[2], targets[0], targets[1], from, to);

                DataPointTXTModel d_txt = new DataPointTXTModel();
                d_txt.target = targets[0] + "." + targets[1] + "." + targets[2];
                d_txt.datapoints = new List<List<string>>();

                foreach (var item in data)
                {
                    d_txt.datapoints.Add(new List<string>() { item.Value.ToString(), item.Time.ToString(), item.Name, item.Comment, item.fillColor });
                }

                dataPointsTXT.Add(d_txt);
            }

            return Json(dataPointsTXT);
        }

        [Route("update/line-status")]
        [HttpPost]
        public IHttpActionResult UpdateLineStatus([FromBody]LineStatusModel query)
        {
            connectionStringName = @query.user.orgName;
            _grafanaCounterRepository.ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            CounterValue item = new CounterValue()
            {
                value = query.datapoint.pointNumber,
                Name = query.datapoint.pointName,
                dt = _dateTimeService.UnixTimeToDateTime(query.datapoint.time),
                fillColor = query.datapoint.fillColor,
                Comment = query.datapoint.commentText
            };

            List<string> target = new List<string>();
            target = query.target.Replace("{", "").Replace("}", "").Split('.').Select(s => s.Trim()).ToList();

            int result = _grafanaCounterRepository.UpdateLineStatus(item, target);

            List<UpdateLineStatusAnswerModel> a = new List<UpdateLineStatusAnswerModel>();

            if (result == 1)
            {
                a.Add(new UpdateLineStatusAnswerModel { target = query.target, statusWrite = "ok" });
            }
            else if (result == -1)
            {
                a.Add(new UpdateLineStatusAnswerModel { target = query.target, statusWrite = "error" });
            }

            return Json(a);
        }

        [Route("write/line-state")]
        [HttpPost]
        public IHttpActionResult WriteLineStatus([FromBody]List<LineWriteModelRead> query)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("[write/line-state] valuesCsv = =*{0}*=", @query[0].connectionStringName);
                log.DebugFormat("[write/line-state] valuesCsv = =*{0}*=", query.ToList().ToString());
            }

            connectionStringName = @query[0].connectionStringName;
            _grafanaCounterRepository.ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            //_grafanaCounterRepository.WriteLineStatus(query[0].typeInfo,_dateTimeService.UnixTimeToDateTime(query[0].dtFrom),query[0].idLine,query[0].idState);

            var items = query
                .Select(r => new LineWriteModelInsert()
                {
                    typeInfo = r.typeInfo,
                    dtFrom = _dateTimeService.UnixTimeToDateTime(r.dtFrom),
                    idLine = r.idLine,
                    idState = r.idState
                }).ToList();

            _grafanaCounterRepository.WriteLinesStatus(items);

            return Ok();
        }

        [Route("write/line-state/csv")]
        [HttpPost]
        public IHttpActionResult WriteLineStatusCSV([FromBody]string valuesCsv)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("[write/line-state/csv] valuesCsv = =*{0}*=", valuesCsv);
            }

            var models = _csvService.FromCSV<LineWriteModelRead>(valuesCsv, true);

            try
            {
                bool res = Int32.TryParse(models.First<LineWriteModelRead>().connectionStringName, out connectionStringNameId);
                if (!res) return Ok();
            }
            catch (Exception)
            {
                return Ok();
            }
            //connectionStringNameId = Int32.Parse(models.First<LineWriteModelRead>().connectionStringName);

            _grafanaCounterRepository.ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringNameId].ToString();

            var items = models
                .Select(r => new LineWriteModelInsert()
                {
                    typeInfo = r.typeInfo,
                    dtFrom = _dateTimeService.UnixTimeToDateTime(r.dtFrom),
                    idLine = r.idLine,
                    idState = r.idState
                });

            _grafanaCounterRepository.WriteLinesStatus(items);

            //string typeInfo = items.First().typeInfo;
            //DateTime dtFrom = new DateTime();
            //dtFrom = _dateTimeService.UnixTimeToDateTime(items.First().dtFrom);
            //int idLine = items.First().idLine;
            //int idState = items.First().idState;

            //string typeInfo = models.First<LineWriteModel>().typeInfo;
            //DateTime dtFrom = new DateTime();
            //dtFrom = _dateTimeService.UnixTimeToDateTime(models.First<LineWriteModel>().dtFrom);
            //int idLine = models.First<LineWriteModel>().idLine;
            //int idState = models.First<LineWriteModel>().idState;

            //switch (Int32.Parse(typeInfo))
            //{
            //    case 1:
            //        typeInfo = "brand";
            //        break;
            //    case 2:
            //        typeInfo = "lineState";
            //        break;
            //    case 3:
            //        typeInfo = "lineMode";
            //        break;
            //}

            //_grafanaCounterRepository.WriteLineStatus(typeInfo, dtFrom, idLine, idState);

            return Ok();
        }

        [Route("values")]
        [HttpPost]
        public IHttpActionResult Post([FromBody]CounterValueInsert counterValues)
        {
            if (counterValues.counterValue.Count() > 0)
            {
                connectionStringName = counterValues.connectionStringName;
                _grafanaCounterRepository.ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

                var items = counterValues.counterValue
                    .Select(r => new CounterValue()
                    {
                        id_counter = r.CounterId,
                        dt = _dateTimeService.UnixTimeToDateTime(r.Time),
                        value = r.Value
                    });

                _grafanaCounterRepository.Insert(items);
            }

            return Ok();
        }

        [Route("values/csv")]
        [HttpPost]
        public void PostCsv([FromBody]string counterValuesCsv)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("[PostCsv] counterValuesCsv = {0}", counterValuesCsv);
            }

            var models = _csvService.FromCSV<CounterValueInsertModel>(counterValuesCsv, true);
            var items = models.Skip<CounterValueInsertModel>(1)
                .Select(r => new CounterValue()
                {
                    id_counter = r.CounterId,
                    dt = _dateTimeService.UnixTimeToDateTime(r.Time),
                    value = r.Value
                });

            connectionStringNameId = (int)models.First<CounterValueInsertModel>().CounterId;
            //_grafanaCounterRepository.ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            _grafanaCounterRepository.ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringNameId].ToString();

            _grafanaCounterRepository.Insert(items);
        }

        [Route("tree")]
        [HttpPost]
        public IHttpActionResult GetTree([FromBody]User query)
        {
            string[] s = @query.orgName.Split('.');

            connectionStringName = s[0];

            _grafanaCounterRepository.ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            var result1 = new List<Werk>();
            var result2 = new List<Line>();
            var result3 = new List<Entities.Counter>();

            Tree tree;

            if (s.Count() > 1)
            {
                if (s[1].Trim().Count() > 1)
                {
                    if (s[1] == "all")
                    {
                        result1 = _grafanaCounterRepository.GetWerks() as List<Werk>;
                        result2 = _grafanaCounterRepository.GetLines() as List<Line>;
                        result3 = _grafanaCounterRepository.GetCounters() as List<Entities.Counter>;
                    }
                    else
                    {
                        result1.Add(_grafanaCounterRepository.GetWerkId(s[1].Trim()) as Werk);
                        result2 = _grafanaCounterRepository.GetLines(s[1].Trim()) as List<Line>;
                        result3 = _grafanaCounterRepository.GetCounters(s[1].Trim()) as List<Entities.Counter>;
                    }

                    tree = new Tree();

                    if (result1.Count > 0 && result2.Count > 0)
                    {
                        List<OrgStructure> orgStructure = result1
                                    .Select(r => new OrgStructure()
                                    {
                                        Id = "c" + r.id,
                                        ParentId = null,
                                        Name = r.name.Trim()
                                    })
                                    .ToList();

                        List<OrgStructure> resultJson2 = result2
                                    .Select(r => new OrgStructure()
                                    {
                                        Id = r.id.ToString(),
                                        ParentId = r.id_werk,
                                        Name = r.name.Trim()
                                    })
                                    .ToList();

                        foreach (OrgStructure item in resultJson2)
                        {
                            orgStructure.Add(item);
                        }

                        tree.OrgStructure = orgStructure;
                    }
                    else
                    {
                        tree.OrgStructure = new List<OrgStructure>();
                    }

                    if (result3.Count > 0)
                    { 
                        List<Entities.Counter> counters = result3
                                    .Select(r => new Entities.Counter()
                                    {
                                        Id = r.Id,
                                        LineId = r.LineId,
                                        Name = r.Name.Trim(),
                                        Color = r.Color.Trim(),
                                        ISO = r.ISO.Trim(),
                                        Min = r.Min,
                                        Max = r.Max
                                    })
                                    .ToList();

                        tree.Counters = counters;
                    }
                    else
                    {
                        tree.Counters = new List<Counter>();
                    }

                    //result1.Clear();
                    //result2.Clear();
                    //result3.Clear();
                }
                else
                {
                    tree = new Tree()
                    {
                        OrgStructure = new List<OrgStructure>(),
                        Counters = new List<Counter>()
                    };
                }
            }
            else
            {
                tree = new Tree()
                {
                    OrgStructure = new List<OrgStructure>(),
                    Counters = new List<Counter>()
                };
            }

            return Json(tree);
        }

        [Route("annotations")]
        [HttpPost]
        public IHttpActionResult Annotations([FromBody]RequestAnnotationModel query)
        {
            string SQL = string.Empty;
            long from;
            long to;
            int counterId = int.Parse(query.annotation.query.Substring(0, query.annotation.query.IndexOf(' ')));

            //connectionStringName = @query.ConnectionStringName;
            _grafanaCounterRepository.ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            from = _dateTimeService.DateTimeToUnixTime(DateTime.Parse(query.range.from));
            to = _dateTimeService.DateTimeToUnixTime(DateTime.Parse(query.range.to));

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("[PostQuery] from = {0}", from);
                log.DebugFormat("[PostQuery] to = {0}", to);
            }

            var counters = _grafanaCounterRepository.GetCounterAnnotations(counterId, DateTime.Parse(query.range.from), DateTime.Parse(query.range.to));
            var countersJson = counters
                .Select(c => new ResponceAnnotationModel()
                {
                    annotation = new AnnotationResp()
                    {
                        name = query.annotation.name,
                        enabled = true,
                        datasource = query.annotation.datasource
                    },
                    title = "",
                    time = _dateTimeService.DateTimeToUnixTime(c.dt),
                    text = c.annotation,
                    tags = ""
                })
                .ToList();

            return Json(countersJson);
        }

        #endregion

        #region Helpers
        private List<CounterValueModel> GetCounterValues(int counterId, long timeFrom, long timeTo)
        {
            var counterValues = _grafanaCounterRepository.GetValuesDapper(
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

        private List<CounterValueModel> GetLineValues(string typeLineState, string werkName, string lineName, long timeFrom, long timeTo)
        {
            switch (typeLineState)
            {
                case lineState:
                    {
                        var values = _grafanaCounterRepository.GetLineStateValuesDapper(
                            werkName,
                            lineName,
                            _dateTimeService.UnixTimeToDateTime(timeFrom),
                            _dateTimeService.UnixTimeToDateTime(timeTo));

                        var models = values
                            .Select(c => new CounterValueModel()
                            {
                                Time = _dateTimeService.DateTimeToUnixTime(c.dt),
                                Value = c.value,
                                Name = c.Name,
                                Comment = c.Comment,
                                fillColor = c.fillColor
                            })
                            .ToList();

                        return models;
                    }
                case lineMode:
                    {
                        var values = _grafanaCounterRepository.GetLineModeValuesDapper(
                            werkName,
                            lineName,
                            _dateTimeService.UnixTimeToDateTime(timeFrom),
                            _dateTimeService.UnixTimeToDateTime(timeTo));

                        var models = values
                            .Select(c => new CounterValueModel()
                            {
                                Time = _dateTimeService.DateTimeToUnixTime(c.dt),
                                Value = c.value,
                                Name = c.Name,
                                Comment = c.Comment,
                                fillColor = c.fillColor
                            })
                            .ToList();

                        return models;
                    }
                case brand:
                    {
                        var values = _grafanaCounterRepository.GetBrandValuesDapper(
                            werkName,
                            lineName,
                            _dateTimeService.UnixTimeToDateTime(timeFrom),
                            _dateTimeService.UnixTimeToDateTime(timeTo));

                        var models = values
                            .Select(c => new CounterValueModel()
                            {
                                Time = _dateTimeService.DateTimeToUnixTime(c.dt),
                                Value = c.value,
                                Name = c.Name,
                                Comment = c.Comment,
                                fillColor = c.fillColor
                            })
                            .ToList();

                        return models;
                    }
            }

            return new List<CounterValueModel>();
        }

        private DataPointsModel Filtration(DataPointsModel source, int maxDataPoints, double eps_start, double eps_end, int eps_step_count)
        {
            int n = source.datapoints.Count;
            int counter = 0;
            double eps_step = Math.Abs((eps_end - eps_start)) / eps_step_count;
            double eps = eps_start;

            DataPointsModel d_Flt = new DataPointsModel();
            d_Flt.target = source.target;
            d_Flt.datapoints = new List<List<double>>();

            double avg = 0;
            double avg_delta = 0;

            if (n > 0)
            {
                avg = source.datapoints.Select(p => Math.Abs(p[0])).Average();
                avg_delta = avg * 0.01;

                while (n > maxDataPoints) //Если количество точек за выбранный период больше maxPointCount, то проредить исходный массив
                {
                    int ic = 0; //счетчик
                    double delta = 0; //отклонение зн-я на текущем шаге
                    double delta_prev = eps + 1; //отклонение на предыдущем шаге

                    //Первая точка нового массива равна первой точке исходного массива
                    d_Flt.datapoints.Add(new List<double>() { source.datapoints[0][0], source.datapoints[0][1] });
                    //----
                    int j = 0;
                    for (j = 0; j < n - 2; j++)
                    {
                        double temp = source.datapoints[j + 1][0];

                        if (Math.Abs(temp) <= avg_delta) temp = source.datapoints[j + 1][0] = 0;

                        if (d_Flt.datapoints[ic][0] == 0 && temp == 0)
                        {
                            delta = 0;
                        }
                        else if (d_Flt.datapoints[ic][0] != 0)
                        {
                            delta = 100 * Math.Abs(d_Flt.datapoints[ic][0] - temp) / Math.Abs(d_Flt.datapoints[ic][0]);
                        }
                        else
                        {
                            delta = 100 * Math.Abs(d_Flt.datapoints[ic][0] - temp) / Math.Abs(temp);
                        }

                        if (delta >= eps && delta_prev <= eps)
                        {
                            ic++;
                            d_Flt.datapoints.Add(new List<double>() { source.datapoints[j][0], source.datapoints[j][1] });
                            ic++;
                            d_Flt.datapoints.Add(new List<double>() { source.datapoints[j + 1][0], source.datapoints[j + 1][1] });
                        }
                        else if (delta > eps && delta_prev > eps)
                        {
                            ic++;
                            d_Flt.datapoints.Add(new List<double>() { source.datapoints[j + 1][0], source.datapoints[j + 1][1] });
                        }

                        delta_prev = delta;
                    }

                    if (j + 1 < n) ic++;

                    d_Flt.datapoints.Add(new List<double>() { source.datapoints[n - 1][0], source.datapoints[n - 1][1] });

                    source.datapoints.Clear();

                    foreach (var t in d_Flt.datapoints)
                    {
                        source.datapoints.Add(new List<double>() { t[0], t[1] });
                    }

                    d_Flt.datapoints.Clear();

                    eps = eps + eps_step;
                    if (eps > eps_end) break;

                    counter++;
                    n = source.datapoints.Count();
                }
            }

            return source;
        }

        #endregion
    }
}