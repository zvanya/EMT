using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EMT.Web.Api.Common.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        private readonly IServiceLocator _serviceLocator;

        public BaseApiController(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public IServiceLocator ServiceLocator => _serviceLocator;
        
    }
}
