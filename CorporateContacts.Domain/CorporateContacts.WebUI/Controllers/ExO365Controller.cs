using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;
using CorporateContacts.WebUI.Infrastructure;
using CorporateContacts.WebUI.Models;
using CorporateContacts.WebUI.Util;
using Microsoft.Exchange.WebServices.Data;
using Folder = Microsoft.Exchange.WebServices.Data.Folder;

namespace CorporateContacts.WebUI.Controllers
{

    [Authorize]
    [CheckSessionOutAttribute]
    [CheckUserIsAdmin]
    public class ExO365Controller : Controller
    {
        //
        // GET: /NewSource/
        IFolderRepo folderRepository;
        IFolderFieldRepo ffRepository;
        ICredentialRepo credRepository;
        IAccountRepo accRepository;
        public ExO365Controller(IFolderRepo folderRepo, IFolderFieldRepo ffRepo, ICredentialRepo credRepo,IAccountRepo accRepo)
        {
            folderRepository = folderRepo;
            ffRepository = ffRepo;
            credRepository = credRepo;
            accRepository = accRepo;
        }

        public ActionResult Index(string src = "", string primid = "")
        {
            User userObj = (User)Session["user"];
            var acc = accRepository.Accounts.FirstOrDefault(x => x.ID == userObj.AccountID);
            //credRepository.SetConnectionString(acc.ConnectionString);
            TempData["SelectedMenu"] = "SetupSync";
            var model = new ExLogOnViewModel();
            model.ExistingCredentials = credRepository.Credentials.ToList();
            model.ReturnUrl = "~/ExO365/SelectFolder?src=" + src + "&primid=" + primid;
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(ExLogOnViewModel model, bool hosted = false)
        {
            User userObj = (User)Session["user"];
            var acc = accRepository.Accounts.FirstOrDefault(x => x.ID == userObj.AccountID);
            //credRepository.SetConnectionString(acc.ConnectionString);
            ExchangeService srv = null;
            if (model.SelectedCredentialID != 0)
            {
                var creds = credRepository.Credentials.FirstOrDefault(x => x.ID == model.SelectedCredentialID);
                if (creds.IsHostedExchange)
                {
                    srv = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                    srv.Credentials = new WebCredentials(creds.EmailAddress, creds.Password);
                    srv.AutodiscoverUrl(creds.EmailAddress, RedirectionUrlValidationCallback);                    
                }
                else
                {
                    if (creds.ServerVersion == "2007SP1") srv = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                    else if (creds.ServerVersion == "2010") srv = new ExchangeService(ExchangeVersion.Exchange2010);
                    else if (creds.ServerVersion == "2010SP1") srv = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
                    else srv = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
                    srv.Credentials = new WebCredentials(creds.UserName, creds.Password);
                    srv.AutodiscoverUrl(creds.EmailAddress, RedirectionUrlValidationCallback);                    
                }
                Session["srv"] = srv;
                Session["srvEmail"] = creds.EmailAddress;
            }
            else
            {                
                if (hosted)
                {
                    srv = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                    srv.Credentials = new WebCredentials(model.Credentials.EmailAddress, model.Credentials.Password);
                    srv.AutodiscoverUrl(model.Credentials.EmailAddress, RedirectionUrlValidationCallback);
                    model.Credentials.URL = srv.Url.AbsoluteUri;
                    
                }
                else
                {
                    if (model.Credentials.ServerVersion == "2007SP1") srv = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                    else if (model.Credentials.ServerVersion == "2010") srv = new ExchangeService(ExchangeVersion.Exchange2010);
                    else if (model.Credentials.ServerVersion == "2010SP1") srv = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
                    else srv = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
                    srv.Credentials = new WebCredentials(model.Credentials.UserName, model.Credentials.Password);
                    srv.AutodiscoverUrl(model.Credentials.EmailAddress, RedirectionUrlValidationCallback);
                    model.Credentials.URL = srv.Url.AbsoluteUri;
                }
                Session["srv"] = srv;
                Session["srvEmail"] = model.Credentials.EmailAddress;
                model.Credentials.IsHostedExchange = hosted;
                credRepository.SaveCredential(model.Credentials);
            }
            return Redirect(model.ReturnUrl);
        }

        static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL
            bool result = false;
            Uri redirectionUri = new Uri(redirectionUrl);
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }
            return result;
        }

        public ActionResult SelectFolder(string src = "", string primid = "")
        {
            TempData["SelectedMenu"] = "SetupSync";
            ExchangeService ex = Session["srv"] as ExchangeService;
            bool addingPrimary = false;
            long primarySourceID = 0;
            try { primarySourceID = Convert.ToInt64(primid); }
            catch { }
            if (src == "prim")
            {
                addingPrimary = true;
            }
            if (ex != null)
            {
                var f = Folder.Bind(ex, WellKnownFolderName.MsgFolderRoot);
                var tree = LoadSubFolders(f, new TreeViewFolder());
                var model = new SelectFolderViewModel();
                model.FolderList = tree.ChildFolders.ToList();
                model.AddingPrimary = addingPrimary;
                model.PrimarySourceId = primarySourceID;
                model.SelectedFolderOwnerInfo = Session["srvEmail"] as string;
                return View(model);
            }
            else
            {
                var model = new SelectFolderViewModel();
                model.FolderList = new List<TreeViewFolder>();
                model.AddingPrimary = addingPrimary;
                model.PrimarySourceId = primarySourceID;
                return View(model);
            }
        }





        [HttpPost]
        public ActionResult SelectFolder(SelectFolderViewModel model)
        {
            User userObj = (User)Session["user"];
            if (!string.IsNullOrEmpty(model.SelectedFolderId))
            {
                var f = new CorporateContacts.Domain.Entities.Folder();
                f.AccountID = userObj.AccountID;
                f.SourceID = model.SelectedFolderId;
                f.IsMaster = true;
                f.SourceName = model.SelectedFolderOwnerInfo;
                f.SourceType = "Exchange";
                f.FolderName = model.SelectedFolderName;
                f.FolderType = "Contact";
                f.IsMaster = model.AddingPrimary;
                f = folderRepository.SaveFolder(f);

                if (!model.AddingPrimary)
                {
                    //add the subscription
                    var s = new Subscription();
                    s.AccountID = userObj.AccountID;
                    s.AllowAdditions = model.AllowAdditions;
                    s.IgnoreExisting = model.IgnoreExisting;                    
                    s.SubscriberID = f.ID;
                    s.PrimaryID = model.PrimarySourceId;
                    folderRepository.SaveSubscription(s);
                    //create the matching folderfields for the subsciption.
                    var folderFieldsForPrimary = ffRepository.FolderFields.Where(x => (x.AccountID == userObj.AccountID) && (x.FolderID == model.PrimarySourceId));
                    foreach (var ff in folderFieldsForPrimary.ToList())
                    {
                        var ffObj = FieldsConfigHelper.CreateFolderFieldFromName(ff.FName);
                        ffObj.FolderID = f.ID;
                        ffObj.AccountID = userObj.AccountID;
                        ffObj.MappedFieldID = ff.ID;
                        ffRepository.SaveFolderField(ffObj);
                    }
                    return RedirectToAction("SetupSync", "Admin", new { primarySourceID = model.PrimarySourceId });
                }
                Session["selectedFolderID"] = f.ID;
                return RedirectToAction("selectfields");
            }
            return View(new List<TreeViewFolder>());
        }




        public ActionResult SelectFields()
        {
            var sourceID = (long)Session["selectedFolderID"];
            if (sourceID != 0)
            {
                var list = FieldsConfigHelper.GetFieldForPrimaryConfig();
                return View(list);
            }
            return RedirectToAction("SetupSync", "Admin");
        }

        [HttpPost]
        public ActionResult SelectFields(string hid1)
        {
            User userObj = (User)Session["user"];
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(hid1);
            foreach (var item in obj)
            {
                var ffObj = FieldsConfigHelper.CreateFolderFieldFromName(item);
                var sourceID = (long)Session["selectedFolderID"];
                ffObj.FolderID = sourceID;
                ffObj.AccountID = userObj.AccountID;
                ffRepository.SaveFolderField(ffObj);
            }
            return RedirectToAction("SetupSync", "Admin");
        }

        private TreeViewFolder LoadSubFolders(Folder f, TreeViewFolder subtree)
        {
            var view = new FolderView(100);
            view.Traversal = FolderTraversal.Shallow;
            var findFolderResults = f.FindFolders(view);
            foreach (var item in findFolderResults.Folders)
            {
                var childNode = new TreeViewFolder();
                childNode.Name = item.DisplayName;
                childNode.Id = item.Id.UniqueId;
                var fclass = item.FolderClass;
                if (fclass == null) fclass = "All";
                if (fclass == "IPF.Contact") fclass = "Contact";
                else if (fclass == "IPF.Appointment") fclass = "Appointment";
                else if (fclass == "IPF.Task") fclass = "Task";
                else fclass = "All";
                childNode.FolderClass = fclass;
                if (item.ChildFolderCount > 0)
                {
                    subtree.ChildFolders.Add(LoadSubFolders(item, childNode));
                }
                else
                {
                    subtree.ChildFolders.Add(childNode);
                }
            }
            return subtree;
        }

    }
}
