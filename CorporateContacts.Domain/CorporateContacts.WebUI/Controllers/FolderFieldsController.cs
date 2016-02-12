using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;
using CorporateContacts.WebUI.Infrastructure;
using CorporateContacts.WebUI.Models;

namespace CorporateContacts.WebUI.Controllers
{
    [Authorize]
    [CheckSessionOutAttribute]
    [CheckUserIsAdmin]
    public class FolderFieldsController : Controller
    {

        ICCFolderFieldRepo CCFolderFieldRepository;

        public FolderFieldsController(ICCFolderFieldRepo folderfield)
        {
            CCFolderFieldRepository = folderfield;
        }

        public ActionResult Index()
        {
            TempData["SelectedMenu"] = "FolderField";
            ViewBag.SelectedMenu = "FolderField";
            return View();           
        }

    }
}
