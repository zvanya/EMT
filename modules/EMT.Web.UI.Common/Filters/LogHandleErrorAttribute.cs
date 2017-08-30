using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace EMT.Web.UI.Common.Filters
{
    public class LogHandleErrorAttribute: HandleErrorAttribute
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnException(ExceptionContext filterContext)
        {
            var exception = filterContext.Exception;
            log.Error(exception);

            base.OnException(filterContext);
        }
    }
}