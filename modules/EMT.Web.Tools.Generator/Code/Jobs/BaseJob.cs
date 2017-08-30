using Common.Logging;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace EMT.Web.Tools.Generator.Code.Jobs
{
    public abstract class BaseJob<TJobArgs> : IJob<TJobArgs>
    {
        #region Fields

        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IServiceLocator _serviceLocator;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isStarted;

        #endregion

        #region Constructors

        public BaseJob(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            _isStarted = false;
        }

        #endregion

        #region Protected Members

        protected IServiceLocator ServiceLocator => this._serviceLocator;

        protected abstract void Perform(TJobArgs args);

        #endregion

        #region IJob

        public async void Start(int interval, TJobArgs args, CancellationToken externalToken)
        {
            if (_isStarted == true)
            {
                throw new InvalidOperationException("The job has already been started");
            }

            _cancellationTokenSource = new CancellationTokenSource();
            var internalToken = _cancellationTokenSource.Token;
            _isStarted = true;
            while (internalToken.IsCancellationRequested == false && externalToken.IsCancellationRequested == false)
            {
                try
                {
                    Perform(args);
                    await Task.Delay(interval);
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
            }
            _isStarted = false;
        }

        public void Stop()
        {
            if (_isStarted == false)
            {
                throw new InvalidOperationException("The job has not been started yet");
            }
            _cancellationTokenSource.Cancel();
        }

        public bool IsStarted => this._isStarted;

        #endregion
    }
}