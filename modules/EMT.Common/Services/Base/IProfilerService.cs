using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.Common.Services.Base
{
    public interface IProfilerService
    {
        bool Enabled { get; }
        void Start();
        void Stop();
        IDisposable Step(string name);
    }
}
