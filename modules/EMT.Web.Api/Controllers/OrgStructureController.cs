using EMT.DAL;
using EMT.Web.Api.Common.Controllers;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EMT.Web.Api.Controllers
{
    [RoutePrefix("api/orgstructure")]
    public class OrgStructureController : BaseApiController
    {
        #region Fields

        private readonly IOrgStructureRepository _structureEntityRepository;

        #endregion

        #region Constructors

        public OrgStructureController(IServiceLocator serviceLocator): base(serviceLocator)
        {
            _structureEntityRepository = this.ServiceLocator.GetInstance<IOrgStructureRepository>();
        }

        #endregion

        #region Web Actions

        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            var structureEntities = _structureEntityRepository.GetAll();

            var structureEntitiesJson = structureEntities
                .Select(c => new
                {
                    id = c.Id,
                    parentId = c.ParentId,
                    path = c.Path,
                    name = c.Name,
                    isLine = c.IsLine
                })
                .ToList();

            return Ok(structureEntitiesJson);
        }

        #endregion

    }
}