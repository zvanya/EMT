using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EMT.Web.Tools.Generator.Code.Jobs
{
    public interface IJob<TJobArgs>
    {
        void Start(int interval, TJobArgs args, CancellationToken externalCancel);
        void Stop();
        bool IsStarted { get; }
    }
}