using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http.Filters;

namespace EMT.Web.Api.Common.Filters
{
    public class LogExceptionFilterAttribute: ExceptionFilterAttribute
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext.Exception;
            log.Error(exception);

            base.OnException(actionExecutedContext);
        }
    }
}