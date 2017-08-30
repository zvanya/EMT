using System;
using EMT.Common.Services.Base;
using StackExchange.Profiling;

namespace EMT.Utils.Profiling
{
    public class MiniProfilerService: IProfilerService
    {
        #region IProfilerService

        public bool Enabled => true;

        public void Start()
        {
            MiniProfiler.Start();
        }

        public void Stop()
        {
            MiniProfiler.Stop();
        }

        public IDisposable Step(string name)
        {
            return MiniProfiler.StepStatic(name);
        }

        #endregion
    }    

}