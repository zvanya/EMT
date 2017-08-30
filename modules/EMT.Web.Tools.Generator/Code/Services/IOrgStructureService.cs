using EMT.Web.Tools.Generator.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.Web.Tools.Generator.Code.Services
{
    public interface IOrgStructureService
    {
        IRestResponse<List<OrgStructureUnitModel>> GetUnits();
    }
}
