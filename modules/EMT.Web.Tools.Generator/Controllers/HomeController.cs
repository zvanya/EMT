using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMT.Web.Tools.Generator.Controllers
{
    public class HomeController : Controller
    {
        private readonly IServiceLocator _serviceLocator;

        public HomeController(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}