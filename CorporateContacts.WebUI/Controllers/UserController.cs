using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xobnu.WebUI.Infrastructure;

namespace Xobnu.WebUI.Controllers
{
    [Authorize]
    [CheckSessionOutAttribute]
    public class UserController : Controller
    {
        //
        // GET: /User/
        IAuthProvider authProvider = null;
        public UserController(IAuthProvider auth)
        {
            authProvider = auth;
        }

        public ActionResult SetupSync()
        {
            ViewBag.SelectedMenu = "SetupSync";
            return View();
        }

        public ActionResult SignOut()
        {
            Session["account"] = null;
            Session["user"] = null;
            authProvider.SignOut();
            return Redirect("~/");
        }

    }
}
