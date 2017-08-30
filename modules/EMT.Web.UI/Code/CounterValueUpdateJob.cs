using Common.Logging;
using EMT.Common.Services;
using EMT.Common.Services.Base;
using EMT.DAL;
using EMT.DAL.Sql.Repositories;
using EMT.Web.UI.Hubs;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace EMT.Web.UI.Code
{
    public class CounterValueJob
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly IDateTimeService _dateTimeService = new DateTimeService();

        public static async void CheckForUpdates(CancellationToken cancel)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["counters_board_db"].ConnectionString;
                var databaseSettings = new DefaultDatabaseSettings(connectionString);
                var counterRepository = new SqlCounterRepository(databaseSettings, new DefaultProfilerService());

                var context = GlobalHost.ConnectionManager.GetHubContext<CounterValueHub>();
                var all = context.Clients.All;

                while (cancel.IsCancellationRequested == false)
                {
                    try
                    {
                        var checkTime = _dateTimeService.Now();
                        await Task.Delay(10000);
                        int[] counters = counterRepository.GetCounterIdsAfter(checkTime).ToArray();
                        all.newValues(counters, _dateTimeService.DateTimeToUnixTime(checkTime));
                    }
                    catch(Exception e)
                    {
                        log.Error(e);
                    }
                }
            } 
            catch(Exception e)
            {
                log.Error(e);
            }
        }
    }
}