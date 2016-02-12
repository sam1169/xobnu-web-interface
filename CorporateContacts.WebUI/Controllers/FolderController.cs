using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;
using Xobnu.WebUI.Infrastructure;
using System.IO;
using Xobnu.WebUI.Models;
using Xobnu.WebUI.Util;
using System.Data;
using PagedList;
using Microsoft.Exchange.WebServices.Data;
using Folder = Microsoft.Exchange.WebServices.Data.Folder;
using System.Reflection;
using System.Configuration;


namespace Xobnu.WebUI.Controllers
{
    [Authorize]
    [CheckSessionOutAttribute]
    [CheckUserIsAdmin]
    [HandleError]
    public class FolderController : Controller
    {
        ICCFolderRepo CCFolderRepository;
        IPlanRepo planRepository;
        IFeatureRepo featureRepository;
        ICCFolderFieldRepo CCFieldRepository;
        ICCItemRepo CCItemRepository;
        ICCFieldValuesRepo CCFieldValueRepository;
        IAccountRepo accRepository;
        ICCGroupRepo CCGroupRepository;
        ICCGroupFieldRepo CCGroupFieldRepository;
        ICCLayoutRepo CCLayoutRepository;
        ICCLayoutGroupRepo CCLayoutGroupRepository;
        ICCConnectionsRepo CCConnectinRepository;
        ICredentialRepo CCCredentialRepository;
        ICCFieldMappingsRepo CCFieldMappingsRepository;
        ICCNoteRepo CCNoteRepository;
        ICCHistoryLogRepo CCHistoryLogRepository;
        List<CCFieldValue> ObjFieldValuess = new List<CCFieldValue>();
        private string rand = "00062008-0000-0000-C000-000000000046";
        public static string conectionBeingAddedEmail = "";

        public FolderController(ICCFolderRepo folder, ICCFolderFieldRepo field, ICCItemRepo Item, ICCFieldValuesRepo fieldvalue, IAccountRepo account, 
            ICCGroupRepo group, ICCGroupFieldRepo groupfield, ICCLayoutRepo layout, ICCLayoutGroupRepo layoutgroup, ICCConnectionsRepo subscription, 
            ICredentialRepo credential, ICCFieldMappingsRepo fieldmapping, ICCNoteRepo note, ICCHistoryLogRepo historyLog,IPlanRepo plan,IFeatureRepo feature)
        {
            CCFolderRepository = folder;
            CCFieldRepository = field;
            CCItemRepository = Item;
            CCFieldValueRepository = fieldvalue;
            accRepository = account;
            CCGroupRepository = group;
            CCGroupFieldRepository = groupfield;
            CCLayoutRepository = layout;
            CCLayoutGroupRepository = layoutgroup;
            CCConnectinRepository = subscription;
            CCCredentialRepository = credential;
            CCFieldMappingsRepository = fieldmapping;
            CCNoteRepository = note;
            CCHistoryLogRepository = historyLog;
            planRepository = plan;
            featureRepository = feature;
        }

        public ActionResult Index()
        {

            TempData["SelectedMenu"] = "Folder";
            ViewBag.SelectedMenu = "Folders";
            return View();
        }

        public ActionResult AddFolder()
        {

            Account accountObj = (Account)Session["account"];
            User userObj = (User)Session["user"];

            var accDetails = accRepository.Accounts.Where(aguid => aguid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();
            var planLeval = planRepository.Plans.Where(pid => pid.ID == accDetails.PlanID).FirstOrDefault().PlanLevel;

            var featureQuality = featureRepository.Features.Where(pid => pid.PlanLevel == planLeval & pid.Type == "Sync Calendar").FirstOrDefault().Quantity;

            if (featureQuality == 0)
            {
                ViewBag.CalendarSyncAvailable = "No";
            }
            else
            {
                ViewBag.CalendarSyncAvailable = "Yes";
            }

            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageFolders";
            ViewBag.SelectedMenu = "Folders";            
            return View();
        }

        public ActionResult ViewCredentials()
        {
            Account accountObj = (Account)Session["account"];
            User userObj = (User)Session["user"];
            TempData["SelectedMenu"] = "Folder";
            var timeZone = (string)Session["timeZone"];
            var credentials = CCCredentialRepository.Credentials.Where(aguid => aguid.AccountGUID == accountObj.AccountGUID).ToList();
            return View(credentials);
        }

        public ActionResult DeleteCredential(long id)
        {

            var associateConnections = CCConnectinRepository.CCSubscriptions.Where(creId => creId.CredentialsID == id).ToList();


            var credentials = CCCredentialRepository.Credentials.Where(cid => cid.ID == id).ToList();
            if (credentials != null && associateConnections.Count() == 0)
            {
                //bool res = CCCredentialRepository.DeleteCredential(id);
            }
            else
            {
                // show popup messsage 
            }
            return RedirectToAction("ManageFolders", "Folder");
        }

        public ActionResult EditCredential(long id)
        {
            TempData["SelectedMenu"] = "ManageFolders";
            CredentialEditModel editCredential = new Models.CredentialEditModel();

            var userToEdit = CCCredentialRepository.Credentials.FirstOrDefault(cid => cid.ID == id);
            string password = Encryption.DecryptStringAES(userToEdit.Password, rand);
            userToEdit.Password = password;
            editCredential.CredentialDetails = userToEdit;
            ViewBag.url = userToEdit.URL;
            if (userToEdit == null)
            {
                return RedirectToAction("ViewCredentials", "Folder");
            }
            else
            {
                return View(editCredential);
            }
        }

        [HttpPost]
        public ActionResult EditCredential(CredentialEditModel credentialObj)
        {
            string buttonStatus = credentialObj.ButtonStatus;
            if (buttonStatus == "AutoDiscover")
            {
                EWSCode ec = new EWSCode();
                ConnectionConfig cc = ec.AutoDiscoverConnectionDetails(credentialObj.CredentialDetails.EmailAddress, credentialObj.CredentialDetails.Password);
                try
                {
                    if (cc.url != null)
                    {
                        credentialObj.CredentialDetails.URL = cc.url;
                        ViewBag.Message = "Connection sucess.";
                        ViewBag.Message1 = "";
                        ViewBag.url = cc.url;
                    }
                    else
                    {
                        ViewBag.Message = "";
                        ViewBag.Message1 = "Connection failed. Please check your password and try again.";
                        return View(credentialObj);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "";
                    ViewBag.Message1 = "Connection failed. Please check your password and try again.";
                    return View(credentialObj);
                }
                

                return View(credentialObj);
            }
            else if (buttonStatus == "TestConnection")
            {
                EWSCode ec = new EWSCode();
                string version = ec.StandardConnection(credentialObj.CredentialDetails.EmailAddress, credentialObj.CredentialDetails.Password, credentialObj.CredentialDetails.URL);

                if (version != null)
                {
                    ViewBag.Message = "Connection Sucess";
                    ViewBag.Message1 = "";
                    ViewBag.url = credentialObj.CredentialDetails.URL;
                }
                else
                {
                    ViewBag.Message = "";
                    ViewBag.Message1 = "Connection Fail";

                }

                return View(credentialObj);

            }

            else
            {

                if (ModelState.IsValid)
                {
                    credentialObj.CredentialDetails.Password = Encryption.EncryptStringAES(credentialObj.CredentialDetails.Password, rand);
                    var resp = CCCredentialRepository.SaveCredential(credentialObj.CredentialDetails);
                    return RedirectToAction("ManageCredentials", "Folder");
                }
                else
                {
                    return View(credentialObj);
                }
            }




        }

        [HttpPost]
        public ActionResult AutoDiscoverCredential(Credential credentialObj)
        {
            return View(credentialObj);
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

       
       
      

        public ActionResult AddCredentials(string src = "", long fid = 1)
        {
            
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];
            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageCredentials";

            ExLogOnViewModel model = new ExLogOnViewModel();

            model.ExistingCredentials = CCCredentialRepository.Credentials.Where(aguid => aguid.AccountGUID == accountObj.AccountGUID).ToList();
            model.ExistingConnections = CCConnectinRepository.CCSubscriptions.Where(folderID => folderID.AccountGUID == accountObj.AccountGUID & folderID.SecondaryAccount != "").ToList().GroupBy(x => x.SecondaryAccount).Select(y => y.First()).ToList();
            model.ReturnUrl = "~/Folder/SelectFolder?src=" + "src" + "&primid=" + fid;
            model.selectedFolderIDInhouse = fid;
            // add plan id
            if (userObj != null)
            {
                var account = accRepository.Accounts.Where(aid => aid.ID == userObj.AccountID).FirstOrDefault();
                model.PlanID = account.PlanID;
            }
            else
            {
                model.PlanID = 1;
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult AddCredentials(ExLogOnViewModel model, bool hosted = false)
        {
            EWSCode ewscode = new EWSCode();
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];
            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageCredentials";
            string selectedButton = string.Empty;
            if (model.ExchangeType == 1) { selectedButton = model.SelectedButtonInhouse; }
            else { selectedButton = model.SelectedButton; }
            long credentialID = 0;
            model.ExistingConnections = CCConnectinRepository.CCSubscriptions.Where(folderID => folderID.AccountGUID == accountObj.AccountGUID & folderID.SecondaryAccount != "").ToList().GroupBy(x => x.SecondaryAccount).Select(y => y.First()).ToList();

            if (selectedButton == "auto")
            {
                ConnectionConfig cc = ewscode.AutoDiscoverConnectionDetails(model.Credentials.EmailAddress, model.Credentials.Password);
                //model.ExistingCredentials = CCCredentialRepository.Credentials.Where(aid => aid.AccountID == userObj.AccountID).ToList();
                model.ExistingCredentials = CCCredentialRepository.Credentials.Where(aguid => aguid.AccountGUID == accountObj.AccountGUID).ToList();
                model.ReturnUrl = "~/Folder/SelectFolder?src=" + "src" + "&primid=" + model.selectedFolderID;

                if (cc != null)
                {
                    //save crediantal 
                    credentialID = SaveCredentials(model, selectedButton, hosted, cc.url, cc.version.ToString());

                    ViewBag.url = cc.url;
                    ViewBag.msucess = "Connection successful and saved.";
                    ViewBag.mfail = "";
                    ViewBag.tmessage = "sucess";
                    ViewBag.pass = Encryption.DecryptStringAES(model.Credentials.Password, rand);
                    model.ServerVer = cc.version.ToString();
                    Session["secondaryAccount"] = model.SecondaryAccount;
                    Session["accessType"] = model.AccessType;
                    string accessTypes = Session["accessType"].ToString();
                    Session["NewCredentialObject"] = model;
                }
                else
                {
                    ViewBag.url = "";
                    ViewBag.mfail = "Error: Auto-discover failed";
                    ViewBag.msucess = "";
                    ViewBag.tmessage = "fail";
                    ViewBag.pass = model.Credentials.Password;
                }
                model.CreatedCredentialID = credentialID;
                
                return View(model);
            }
            else if (selectedButton == "test")
            {

                string version = ewscode.StandardConnection(model.Credentials.EmailAddress, model.Credentials.Password, model.Credentials.URL);
                model.ExistingCredentials = CCCredentialRepository.Credentials.Where(aguid => aguid.AccountGUID == accountObj.AccountGUID).ToList();
                model.ReturnUrl = "~/Folder/SelectFolder?src=" + "src" + "&primid=" + model.selectedFolderID;
                if (version != "" && version != "404" && version != "401" && version != "null")
                {
                    //save crediantal 
                    credentialID = SaveCredentials(model, selectedButton, hosted, model.Credentials.URL, version.ToString());

                    ViewBag.url = model.Credentials.URL;
                    ViewBag.pass = Encryption.DecryptStringAES(model.Credentials.Password, rand);
                    ViewBag.msucess = "Connection successful and saved.";
                    ViewBag.mfail = "";
                    ViewBag.tmessage = "sucess";
                    model.ServerVer = version.ToString();

                }
                else
                {
                    ViewBag.url = "";
                    if (version == "") { ViewBag.mfail = "Connection Fail"; }
                    else if (version == "404") { ViewBag.mfail = "Error: The URL was not found. Please check the URL."; }
                    else if (version == "401") { ViewBag.mfail = "Error: Server returned 'Unauthorized'. Please check the username and password"; }
                    else if (version == "null") { ViewBag.mfail = "Error: URL cannot be null. Please enter the URL."; }
                    else { ViewBag.mfail = "Connection Fail"; }
                    ViewBag.msucess = "";
                    ViewBag.tmessage = "fail";
                }

                model.CreatedCredentialID = credentialID;
                return View(model);
            }
            else if (selectedButton == "autoInhouse")
            {
                ConnectionConfig cc = ewscode.AutoDiscoverConnectionDetails(model.EmailAddressIn, model.Password);
                model.ExistingCredentials = CCCredentialRepository.Credentials.Where(aguid => aguid.AccountGUID == accountObj.AccountGUID).ToList();
                model.ReturnUrl = "~/Folder/SelectFolder?src=" + "src" + "&primid=" + model.selectedFolderID;

                if (cc != null)
                {
                    //save crediantal 
                    credentialID = SaveCredentials(model, selectedButton, hosted, cc.url, cc.version.ToString());

                    ViewBag.urlin = cc.url;
                    ViewBag.msucessin = "Connection successful and saved.";
                    ViewBag.mfailin = "";
                    ViewBag.tmessagein = "";
                    ViewBag.passin = model.Password;
                    model.ServerVersionInhouse = cc.version.ToString();
                    Session["secondaryAccount"] = model.SecondaryAccountIn;
                    Session["accessType"] = model.AccessTypeIn;
                }
                else
                {
                    ViewBag.urlin = "";
                    ViewBag.mfailin = "Error: Auto-discover failed";
                    ViewBag.msucessin = "";
                    ViewBag.tmessagein = "";
                    ViewBag.passin = model.Password;
                }
                model.CreatedCredentialIDIn = credentialID;
                return View(model);

            }
            else if (selectedButton == "testInhouse")
            {
                model.ExistingCredentials = CCCredentialRepository.Credentials.Where(aguid => aguid.AccountGUID == accountObj.AccountGUID).ToList();
                string version = ewscode.StandardConnectionToInHouseExchanges(model.UserName, model.Password, model.URL, model.Domain);
                model.ReturnUrlInhouse = "~/Folder/SelectFolder?src=" + "src" + "&primid=" + model.selectedFolderIDInhouse;

                if (version != "" && version != "404" && version != "401" && version != "null")
                {
                    //save crediantal 
                    credentialID = SaveCredentials(model, selectedButton, hosted, model.URL, version.ToString());

                    ViewBag.urlin = model.URL;
                    ViewBag.passin = model.Password;
                    ViewBag.msucessin = "Connection successful and saved.";
                    ViewBag.mfailin = "";
                    ViewBag.tmessagein = "sucess";
                    model.ServerVersionInhouse = version.ToString();

                }
                else
                {

                    ViewBag.urlin = "";
                    if (version == "") { ViewBag.mfailin = "Connection Fail"; }
                    else if (version == "404") { ViewBag.mfailin = "Error: The URL was not found. Please check the URL."; }
                    else if (version == "401") { ViewBag.mfailin = "Error: Server returned 'Unauthorized'. Please check the username and password"; }
                    else if (version == "null") { ViewBag.mfailin = "Error: URL cannot be null. Please enter the URL."; }
                    else { ViewBag.mfailin = "Connection Fail"; }
                    ViewBag.msucessin = "";
                    ViewBag.tmessagein = "fail";
                }
                model.CreatedCredentialIDIn = credentialID;
                return View(model);

            }
            else if (selectedButton == "setUpConnection")
            {
                Session["NewCredential"] = "Yes";
                return RedirectToAction("setUpNewConnection", "Folder");
            }
            else
            {
                Session["NewCredential"] = "Yes";
                return RedirectToAction("ManageCredentials", "Folder");
            }
        }


        public long SaveCredentials(ExLogOnViewModel model, string selectedButton, bool hosted, string url, string version)
        {
            long credentialID = 0;
            string enpassword = string.Empty;
            Account accountObj = (Account)Session["account"];

            Credential resp = new Credential();
            if (selectedButton == "auto" || selectedButton == "test")
            {
                enpassword = Encryption.EncryptStringAES(model.Credentials.Password, rand);
                Session["srvEmail"] = model.Credentials.EmailAddress;
                model.Credentials.IsHostedExchange = hosted;
                model.Credentials.Password = enpassword;
                model.Credentials.ServerVersion = version;
                model.Credentials.URL = url;
                model.Credentials.AccountGUID = accountObj.AccountGUID;
                resp = CCCredentialRepository.SaveCredential(model.Credentials);
                credentialID = resp.ID;
            }
            else
            {
                enpassword = Encryption.EncryptStringAES(model.Password, rand);
                Credential credintialIn = new Credential();
                credintialIn.Name = model.Name;
                if (model.UserName != null) { credintialIn.UserName = model.UserName; Session["srvEmail"] = model.UserName; }
                else { credintialIn.EmailAddress = model.EmailAddressIn; Session["srvEmail"] = model.EmailAddressIn; }
                credintialIn.URL = url;
                credintialIn.Password = enpassword;
                credintialIn.ServerVersion = version;
                credintialIn.IsHostedExchange = false;
                credintialIn.Domain = model.Domain;
                credintialIn.AccountGUID = accountObj.AccountGUID;
                resp = CCCredentialRepository.SaveCredential(credintialIn);
                credentialID = resp.ID;
            }

            return credentialID;
        }

        

      
        public ActionResult ManageCredentials()
        {
            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageCredentials";
            Account accountObj = (Account)Session["account"];

            Session["NewCredentialObject"] = null;
            ManageCredentialsViewModel manageCredentials = new ManageCredentialsViewModel();
            List<Credential> credentials = CCCredentialRepository.Credentials.Where(guid => guid.AccountGUID == accountObj.AccountGUID).ToList();
            manageCredentials.Credentials = credentials;

            return View(manageCredentials);
        }


        public ActionResult ManageLayoutsAndGroups(long ID)
        {
            TempData["SelectedMenu"] = "ManageFolders";
            ManageLayoutsAndGroupsViewModel manageLayoutsAndGroups = new ManageLayoutsAndGroupsViewModel();
            Account accountObj = (Account)Session["account"];

            var groups = CCGroupRepository.CCGroups.Where(u => u.FolderID == ID & u.AccountGUID == accountObj.AccountGUID).ToList();
            manageLayoutsAndGroups.Groups = groups;

            var layouts = CCLayoutRepository.CCLayouts.Where(u => u.FolderID == ID & u.AccountGUID == accountObj.AccountGUID).ToList();
            if (layouts.Count == 0)
            {
                List<CCLayout> layoutlist = new List<CCLayout>();
                CCLayout layoutobj = new CCLayout();
                layoutobj.FolderID = ID;
                layoutobj.LayoutID = 0;
                layoutobj.LayoutName = "empty";
                layoutlist.Add(layoutobj);
                manageLayoutsAndGroups.Layouts = layoutlist;
            }
            else
            {
                manageLayoutsAndGroups.Layouts = layouts;
            }

            var folderNmae = CCFolderRepository.CCFolders.Where(fid => fid.FolderID == ID & fid.AccountGUID == accountObj.AccountGUID).FirstOrDefault().Name;
            manageLayoutsAndGroups.FolderName = folderNmae;

            return View(manageLayoutsAndGroups);
        }

        public ActionResult DeleteUnAssociateCredential(long ID)
        {
            Account accountObj = (Account)Session["account"];
            string connections = string.Empty;

            var associateConnections = CCConnectinRepository.CCSubscriptions.Where(creId => creId.CredentialsID == ID & creId.AccountGUID == accountObj.AccountGUID).ToList();


            var credentials = CCCredentialRepository.Credentials.Where(cid => cid.ID == ID & cid.AccountGUID == accountObj.AccountGUID).ToList();
            if (credentials != null && associateConnections.Count() == 0)
            {
                connections = "No";
                bool res = CCCredentialRepository.DeleteCredential(ID);
            }
            else
            {
                foreach (var conn in associateConnections)
                {
                    connections += conn.FolderName + " + ";
                }

                connections = connections.Remove(connections.Length - 2);
            }

            return Json(connections, JsonRequestBehavior.AllowGet);



        }

      
     

        public string[] seperateDeDupeValue(string val)
        {
            string[] values = val.Split('|');
            return values;
        }

        public string getTitleForItem(CCItems item)
        {
            string[] seperatedValues = seperateDeDupeValue(item.DeDupeValue);

            if (seperatedValues.Count() == 4)
            {
                if (seperatedValues[0] != "" && seperatedValues[2] != "")
                {
                    return seperatedValues[0] + " " + seperatedValues[2] + " - " + seperatedValues[3];
                }
                else
                {
                    return item.ItemID.ToString();
                }
            }
            else if (seperatedValues.Count() == 3)
            {
                if (seperatedValues[0] != "")
                {
                    return seperatedValues[0];
                }
                else
                {
                    return item.ItemID.ToString();
                }
            }
            return "";
        }

   

    


       


      


      


    }
}
