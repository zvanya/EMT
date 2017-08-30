using EMT.Common.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.Common.Services
{
    public class DefaultProfilerService : IProfilerService
    {
        private class EmptyDisposable : IDisposable
        {
            public void Dispose()
            {                
            }
        }

        public bool Enabled => false;

        public IDisposable Step(string name)
        {
            return new EmptyDisposable();
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}
