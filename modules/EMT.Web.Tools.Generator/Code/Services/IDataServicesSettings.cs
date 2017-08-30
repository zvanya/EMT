using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.Web.Tools.Generator.Code.Services
{
    public interface IDataServicesSettings
    {
        string CountersEntpoint { get; }
        string CounterValuesReceiveEntpoint { get; }
        string OrgStructureEntpoint { get; }
    }
}
