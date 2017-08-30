using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.IO;

namespace EMT.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            string indexPageFile = this.Server.MapPath("~/pages/index.html");
            string indexPageContent = System.IO.File.ReadAllText(indexPageFile);
            return Content(indexPageContent, "text/html");
        } 
    }
}
