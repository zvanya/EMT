using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EMT.Web.Tools.Generator.Models;
using RestSharp;

namespace EMT.Web.Tools.Generator.Code.Services
{
    public class OrgStructureService : IOrgStructureService
    {
        #region Fields

        private readonly IDataServicesSettings _dataServicesSettings;        

        #endregion

        #region Constructors

        public OrgStructureService(IDataServicesSettings dataServicesSettings)
        {
            _dataServicesSettings = dataServicesSettings;            
        }

        #endregion

        #region IOrgStructureService

        public IRestResponse<List<OrgStructureUnitModel>> GetUnits()
        {
            var client = new RestClient(_dataServicesSettings.OrgStructureEntpoint);

            var request = new RestRequest("", Method.GET);

            var response = client.Execute<List<OrgStructureUnitModel>>(request);

            return response;
        }

        #endregion
    }
}