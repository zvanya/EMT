using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;

namespace EMT.Web.Tools.Generator.Code.Services
{
    public class DataServicesSettings: IDataServicesSettings
    {
        private readonly NameValueCollection _appSettings;

        public DataServicesSettings()
        {
            _appSettings = ConfigurationManager.AppSettings;
        }

        public string CountersEntpoint => _appSettings["CountersEntpoint"];

        public string CounterValuesReceiveEntpoint => _appSettings["CounterValuesReceiveEntpoint"];

        public string OrgStructureEntpoint => _appSettings["OrgStructureEntpoint"];
    }
}