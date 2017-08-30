using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace EMT.Web.Tools.Generator.Code.Jobs
{
    public class CounterJobManager
    {
        #region Fields

        private readonly IServiceLocator _serviceLocator;
        private readonly Dictionary<int, CounterJob> _jobs;

        #endregion

        #region Constructors

        public CounterJobManager(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            _jobs = new Dictionary<int, CounterJob>();
        }

        #endregion

        #region Public Members

        public void Start(int counterId, int interval, CounterJobArgs args)
        {
            CounterJob job = this.GetOrCreateJob(counterId);
            if (job.IsStarted == true)
            {
                throw new InvalidOperationException("The job has already been started");
            }
            HostingEnvironment.QueueBackgroundWorkItem(cxt => Task.Run(() => job.Start(interval, args, cxt)));
        }

        public void Stop(int counterId)
        {
            if (this.IsStarted(counterId) == false)
            {
                throw new InvalidOperationException("The job has not been started yet");
            }
            CounterJob job = this.GetJob(counterId);
            job.Stop();
        }

        public bool IsStarted(int counterId)
        {
            return this.IsJob(counterId) && this.GetJob(counterId).IsStarted;
        }

        #endregion

        #region Helpers

        private bool IsJob(int counterId)
        {
            return _jobs.ContainsKey(counterId);
        }

        private CounterJob GetOrCreateJob(int counterId)
        {
            if (_jobs.ContainsKey(counterId) == false)
            {
                _jobs[counterId] = this.CreateJob(counterId);
            }
            return _jobs[counterId];
        }

        private CounterJob GetJob(int counterId)
        {
            return _jobs[counterId];
        }        

        private CounterJob CreateJob(int counterId)
        {
            return new CounterJob(counterId, _serviceLocator);
        }

        #endregion
    }
}