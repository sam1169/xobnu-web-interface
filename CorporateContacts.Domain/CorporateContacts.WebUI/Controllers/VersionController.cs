using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;

namespace CorporateContacts.WebUI.Controllers
{
    public class VersionController : Controller
    {
        //
        // GET: /Version/

        public ActionResult Index()
        {
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ViewBag.version = assemblyVersion;
            return View();
        }

    }
}
