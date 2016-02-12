using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;
using CorporateContacts.WebUI.Infrastructure;
using System.IO;
using CorporateContacts.WebUI.Models;
using CorporateContacts.WebUI.Util;
using System.Data;
using PagedList;
using Microsoft.Exchange.WebServices.Data;
using Folder = Microsoft.Exchange.WebServices.Data.Folder;
using System.Reflection;
using System.Configuration;


namespace CorporateContacts.WebUI.Controllers
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

        [HttpPost]
        public ActionResult EditFolder(CCFolder folderObj)
        {
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];
            CCFolder folderResponse;
            int folderType = folderObj.Type;

            if (ModelState.IsValid && accountObj != null)
            {
                folderObj.AccountGUID = accountObj.AccountGUID;
                folderObj.IsPaused = false;
                folderObj.isOverFlow = false;
                if (folderObj.Type == 3)
                    folderObj.isCrimeDiary = true;
                //Save Folder                
                folderResponse = CCFolderRepository.SaveFolder(folderObj);

                var folders = CCFolderRepository.CCFolders.Where(guid => guid.AccountGUID == accountObj.AccountGUID).ToList();
                TempData["SelectedMenu"] = "Manage";
                TempData["SelectedSubMenu"] = "ManageFolders";

                if (folderObj.FolderID > 0)
                {
                    var fields = CCFieldRepository.CCFolderFields.Where(u => u.FolderID == folderObj.FolderID & u.AccountGUID == accountObj.AccountGUID).ToList();
                    if (fields.Count == 0)
                    {
                        //Save default field                       
                        List<string> needtosave = new List<string>();
                        List<CCFolderField> savedobj = new List<CCFolderField>();

                        if (folderType == 1) {
                            if (accountObj.PlanID == 1)
                            {
                                needtosave = FieldsConfigHelper.GetFieldForContactSimple(); 
                            }
                            else if (accountObj.PlanID == 2)
                            {
                                needtosave = FieldsConfigHelper.GetFieldForContactBusiness(); 
                            }
                            else
                            {
                                needtosave = FieldsConfigHelper.GetFieldForContactFull(); 
                            }
                        }
                        else if (folderType == 2) { needtosave = FieldsConfigHelper.GetFieldForAppointmentSimple(); }
                        else if (folderType == 3) { needtosave = FieldsConfigHelper.GetFieldForCrimeDiaryAppointment(); }
                        else { }

                        foreach (var field in needtosave)
                        {
                            CCFolderField folderField = new CCFolderField();
                            folderField.FieldName = FieldsConfigHelper.GetRealName(field);
                            folderField.FieldType = FieldsConfigHelper.GetVariableType(field);
                            folderField.FolderID = folderObj.FolderID;
                            folderField.FieldCaption = field;
                            folderField.Restriction = "none";
                            folderField.AccountGUID = accountObj.AccountGUID;
                            folderField.isActive = true;
                            savedobj.Add(folderField);
                        }

                        var resp = CCFieldRepository.SaveFolderFieldsObj(savedobj);

                    }

                    // create note into grops                 
                    var alreadyExist = CCGroupRepository.CCGroups.Where(fid => fid.FolderID == folderResponse.FolderID & fid.GroupName == "Note" & fid.AccountGUID == accountObj.AccountGUID).ToList();

                    if (alreadyExist.Count == 0)
                    {
                        CCGroup objGroup = new CCGroup();
                        objGroup.GroupName = "Note";
                        objGroup.FolderID = folderResponse.FolderID;
                        objGroup.AccountGUID = accountObj.AccountGUID;
                        var user = CCGroupRepository.SaveGroup(objGroup);
                    }

                    // create default group
                    var isDefaultGrpExist = CCGroupRepository.CCGroups.Where(fid => fid.FolderID == folderResponse.FolderID & fid.GroupName == "Default" & fid.AccountGUID == accountObj.AccountGUID).ToList();

                    if (isDefaultGrpExist.Count == 0)
                    {
                        // add default group 
                        CCGroup objGroup = new CCGroup();
                        objGroup.GroupName = "Default";
                        objGroup.FolderID = folderResponse.FolderID;
                        objGroup.AccountGUID = accountObj.AccountGUID;
                        var grp = CCGroupRepository.SaveGroup(objGroup);

                        if (grp.GroupID != 0)
                        {
                            // added all default field                           
                            var defaultfields = CCFieldRepository.CCFolderFields.Where(u => u.FolderID == folderObj.FolderID & u.AccountGUID == accountObj.AccountGUID).ToList();
                            int i = 1;
                            foreach (var fieldItem in defaultfields)
                            {
                                CCGroupField objgrpfield = new CCGroupField();
                                objgrpfield.FieldID = fieldItem.FieldID;
                                objgrpfield.FieldOrder = i++;
                                objgrpfield.FolderID = grp.FolderID;
                                objgrpfield.GroupID = grp.GroupID;
                                objgrpfield.AccountGUID = accountObj.AccountGUID;
                                var aa = CCGroupFieldRepository.SaveGroupField(objgrpfield);
                            }
                        }
                    }

                    // create default layout                  
                    var isDefaultLayoutExist = CCLayoutRepository.CCLayouts.Where(fid => fid.FolderID == folderResponse.FolderID & fid.LayoutName == "Default" & fid.AccountGUID == accountObj.AccountGUID).ToList();

                    if (isDefaultLayoutExist.Count == 0)
                    {
                        // add default layout
                        CCLayout layout = new CCLayout();
                        layout.LayoutName = "Default";
                        layout.FolderID = folderObj.FolderID;
                        layout.AccountGUID = accountObj.AccountGUID;
                        var resLayout = CCLayoutRepository.Savelayout(layout);

                        // add default group into default layout                        
                        var defaultExistGrp = CCGroupRepository.CCGroups.Where(fid => fid.FolderID == folderResponse.FolderID & fid.GroupName == "Default" & fid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();
                        if (defaultExistGrp != null)
                        {
                            CCLayoutGroup objLayoutGroup = new CCLayoutGroup();
                            objLayoutGroup.GroupID = defaultExistGrp.GroupID;
                            objLayoutGroup.GroupOrder = 1;
                            objLayoutGroup.FolderID = resLayout.FolderID;
                            objLayoutGroup.LayoutID = resLayout.LayoutID;
                            objLayoutGroup.AccountGUID = accountObj.AccountGUID;
                            var aa = CCLayoutGroupRepository.SavelayoutGroup(objLayoutGroup);
                        }
                    }

                }

                var folderList = CCFolderRepository.CCFolders.Where(guid => guid.AccountGUID == accountObj.AccountGUID).ToList();
                Session["folderss"] = folderList;

                LimitationsViewModel limitationsObjMain = (LimitationsViewModel)Session["limitations"];
                HelperFunctions HF = new HelperFunctions();
                limitationsObjMain = HF.updateAccountLimitations(accountObj);
                Session["limitations"] = limitationsObjMain;

                return RedirectToAction("ManageFolders", "Folder");
            }
            else
            {
                var folderList = CCFolderRepository.CCFolders.Where(guid => guid.AccountGUID == accountObj.AccountGUID).ToList();
                Session["folderss"] = folderList;

                LimitationsViewModel limitationsObjMain = (LimitationsViewModel)Session["limitations"];
                HelperFunctions HF = new HelperFunctions();
                limitationsObjMain = HF.updateAccountLimitations(accountObj);
                Session["limitations"] = limitationsObjMain;

                return View(folderObj);
            }
        }

        public ActionResult EditFolder(long ID)
        {
            Account accountObj = (Account)Session["account"];
            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageFolders";
            var userToEdit = CCFolderRepository.CCFolders.FirstOrDefault(u => u.FolderID == ID & u.AccountGUID == accountObj.AccountGUID);
            if (userToEdit == null)
            {
                return View(new User());
            }
            else return View(userToEdit);
        }

        [HttpPost]
        public ActionResult Deletefolder(long folderID)
        {
            Account accountObj = (Account)Session["account"];
            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageFolders";
            var userToDelete = CCFolderRepository.CCFolders.FirstOrDefault(u => u.FolderID == folderID & u.AccountGUID == accountObj.AccountGUID);
            if (userToDelete != null)
            {
                bool result = CCFolderRepository.DeleteFolder(folderID);
            }

            return RedirectToAction("Viewfolder");
        }

        public ActionResult Field(long id)
        {
            return View(new CCFolderField { FolderID = id });
        }

        [HttpPost]
        public ActionResult Field(CCFolderField folderObj)
        {

            if (CCFieldRepository.IsFieldAvailable(folderObj.FieldName, folderObj.FolderID) || folderObj.FieldID > 0)
            {
                User userObj = (User)Session["user"];
                Account accountObj = (Account)Session["account"];

                if (folderObj.Restriction == "none")
                    folderObj.RestrictionValues = null;
                if (ModelState.IsValid)
                {
                    folderObj.AccountGUID = accountObj.AccountGUID;
                    folderObj.isActive = true;
                    var resField = CCFieldRepository.SaveFolderFields(folderObj);
                    if (resField.FieldID != 0)
                    {
                        // add created field into Default group                       
                        var isDefaultExist = CCGroupRepository.CCGroups.Where(fid => fid.FolderID == folderObj.FolderID & fid.GroupName == "Default" & fid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();
                        if (isDefaultExist != null)
                        {
                            //is field exist into default
                            var isFieldexist = CCGroupFieldRepository.CCGroupsFields.Where(fid => fid.FieldID == resField.FieldID & fid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();
                            if (isFieldexist == null)
                            {
                                // get group fileds last order
                                var groupFieldOrder = CCGroupFieldRepository.CCGroupsFields.Where(gid => gid.GroupID == isDefaultExist.GroupID & gid.AccountGUID == accountObj.AccountGUID).OrderByDescending(grp => grp.FieldOrder).FirstOrDefault();

                                CCGroupField objgrpfield = new CCGroupField();
                                objgrpfield.FieldID = resField.FieldID;
                                objgrpfield.FieldOrder = groupFieldOrder.FieldOrder + 1;
                                objgrpfield.FolderID = isDefaultExist.FolderID;
                                objgrpfield.GroupID = isDefaultExist.GroupID;
                                objgrpfield.AccountGUID = accountObj.AccountGUID;
                                var resGrpField = CCGroupFieldRepository.SaveGroupField(objgrpfield);
                            }
                        }
                    }
                    return RedirectToAction("ViewFields", "Folder", new { id = folderObj.FolderID });
                }
                else
                {
                    return View(folderObj);
                }
            }
            else
            {
                ViewBag.Message = "That field is already in this folder";
                return View(folderObj);
            }

        }

        public ActionResult FieldMessage(long id)
        {
            return View(new CCFolderField { FolderID = id });
        }

        public ActionResult ViewFields(long ID)
        {
            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageFolders";
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];

            var fields = CCFieldRepository.CCFolderFields.Where(u => u.FolderID == ID & u.AccountGUID == accountObj.AccountGUID & u.isActive == true).ToList();

            List<String> objFields = new List<string>();

            CCFolder folderDetails = null;
            folderDetails = CCFolderRepository.FolderDetails(ID);

            if (folderDetails.Type == 1)
            {
                if (accountObj.PlanID == 1)
                    objFields = FieldsConfigHelper.GetFieldForContactSimple();
                else if (accountObj.PlanID == 2)
                    objFields = FieldsConfigHelper.GetFieldForContactBusiness();
                else
                    objFields = FieldsConfigHelper.GetFieldForContactFull();
            }
            else
            { 
                if(folderDetails.isCrimeDiary == true)
                    objFields = FieldsConfigHelper.GetFieldForCrimeDiaryAppointment();
                else
                    objFields = FieldsConfigHelper.GetFieldForAppointmentSimple();
            }

            


            foreach (var field in fields)
            {
                foreach (var ccfield in objFields)
                {
                    if (ccfield == field.FieldCaption)
                    {
                        field.isMandatory = true;
                        break;
                    }
                }
            }

            AddContactViewModel folderFields = new AddContactViewModel();
            folderFields.FolderFields = fields;

            int type = 1;
            

            
            if (folderDetails != null) { type = folderDetails.Type; }
            else { type = 1; }
            folderFields.FolderType = type;

            if (fields.Count == 0)
            {
                return RedirectToAction("FieldMessage", "Folder", new { id = ID });
            }
            else
            {



                return View(folderFields);
            }
                
        }

        public ActionResult EditFolderField(long id, long fid)
        {
            Account accountObj = (Account)Session["account"];
            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageFolders";
            var selectedfield = CCFieldRepository.CCFolderFields.Where(feldid => feldid.FieldID == id & feldid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();

            if (selectedfield == null)
            {
                return RedirectToAction("ViewFields", "Folder", new { id = fid });
            }
            else return View(selectedfield);

        }

        public ActionResult Deletefield(long id, long fid)
        {
            if (id > 0)
            {
                Account accountObj = (Account)Session["account"];
                bool result = CCFieldRepository.DeleteField(id);
                if (result)
                {
                    // Delete field from all groups                    
                    var allGroups = CCGroupRepository.CCGroups.Where(folderid => folderid.FolderID == fid & folderid.AccountGUID == accountObj.AccountGUID).ToList();
                    foreach (var group in allGroups)
                    {
                        var isExistField = CCGroupFieldRepository.CCGroupsFields.Where(grp => grp.GroupID == group.GroupID && grp.FieldID == id && grp.FolderID == fid && grp.AccountGUID == accountObj.AccountGUID).FirstOrDefault();
                        if (isExistField != null)
                        {
                            var resDeleteGrpField = CCGroupFieldRepository.DeleteGroupField(isExistField.GrpFieldID);
                        }
                    }

                }
            }

            return RedirectToAction("ViewFields", "Folder", new { id = fid });
        }

        public ActionResult SelectFolderFields(long id)
        {
            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageFolders";
            Account accountObj = (Account)Session["account"];
            FolderFieldsSelectModel folderField = new FolderFieldsSelectModel();

            var type = CCFolderRepository.CCFolders.FirstOrDefault(fid => fid.FolderID == id & fid.AccountGUID == accountObj.AccountGUID).Type;

            var savedFields = CCFieldRepository.CCFolderFields.Where(fid => fid.FolderID == id & fid.AccountGUID == accountObj.AccountGUID & fid.isActive == true).Select(fname => fname.FieldCaption).ToList();
            if (type == 1)
            {
                folderField.SelectFolderFieldsSchemas = FieldsConfigHelper.GetFieldForPrimaryConfig();
            }
            else if (type == 2)
            {
                folderField.SelectFolderFieldsSchemas = FieldsConfigHelper.GetFieldForAppointmentConfig();
            }
            else
            { }

            folderField.FolderID = id;
            folderField.SavedFields = savedFields;
            folderField.Type = type;

            return View(folderField);
        }

        [HttpPost]
        public ActionResult SelectFolderFields(string fields, FolderFieldsSelectModel objFolderFields)
        {
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];
            List<string> needtosave = new List<string>();
            List<CCFolderField> savedobj = new List<CCFolderField>();
            List<CCFolderField> activeFields = new List<CCFolderField>();
            List<CCFolderField> needToInActive = new List<CCFolderField>();

            var objFields = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(fields).ToList();
            needtosave = CCFieldRepository.IsAvailableField(objFields, objFolderFields.FolderID);

            


            foreach (var field in needtosave)
            {
                CCFolderField folderField = new CCFolderField();
                folderField.FieldName = FieldsConfigHelper.GetRealName(field);
                folderField.FieldType = FieldsConfigHelper.GetVariableType(field);
                folderField.FolderID = objFolderFields.FolderID;
                folderField.FieldCaption = field;
                folderField.Restriction = "none";
                folderField.AccountGUID = accountObj.AccountGUID;
                folderField.isActive = true;
                savedobj.Add(folderField);
            }

            foreach (var field in objFields)
            {
                CCFolderField folderField = CCFieldRepository.CCFolderFields.Where(f => f.FieldCaption == field & f.AccountGUID == accountObj.AccountGUID & f.FolderID == objFolderFields.FolderID).FirstOrDefault();
                activeFields.Add(folderField);
            }
                
            var resp = CCFieldRepository.SaveFolderFieldsObj(savedobj);

            var fieldList = CCFieldRepository.CCFolderFields.Where(f => f.AccountGUID == accountObj.AccountGUID & f.FolderID == objFolderFields.FolderID).ToList();

            var toInActiveList = fieldList.Except(activeFields).ToList();

            int flag = 0;

            foreach (var f1 in fieldList)
            {
                flag = 0;
                foreach (var f2 in activeFields)
                {
                    try
                    {
                        if (f2.FieldID == f1.FieldID)
                        {
                            if (f1.isActive != false)
                            {
                                flag = 1;
                                break;
                            }
                            else
                            {
                                flag = 2;
                                break;
                            }

                        }
                    }
                    catch (Exception ex)
                    {  }
                    
                }

                if (flag == 0)
                {
                    f1.isActive = false;
                    CCFieldRepository.SaveFolderFields(f1);
                }

                else if (flag == 2)
                {
                    f1.isActive = true;
                    CCFieldRepository.SaveFolderFields(f1);
                }
            }

            // add field into default group            
            var defaultGrp = CCGroupRepository.CCGroups.Where(fid => fid.FolderID == objFolderFields.FolderID & fid.GroupName == "Default" & fid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();

            if (defaultGrp != null)
            {
                foreach (var field in resp)
                {
                    long grpFieldOrder = 0;
                    // get group fileds last order
                    var groupFieldOrder = CCGroupFieldRepository.CCGroupsFields.Where(gid => gid.GroupID == defaultGrp.GroupID & gid.AccountGUID == accountObj.AccountGUID).OrderByDescending(grp => grp.FieldOrder).FirstOrDefault();
                    if (groupFieldOrder != null) { grpFieldOrder = groupFieldOrder.FieldOrder; }

                    CCGroupField objgrpfield = new CCGroupField();
                    objgrpfield.FieldID = field.FieldID;
                    objgrpfield.FieldOrder = grpFieldOrder + 1;
                    objgrpfield.FolderID = defaultGrp.FolderID;
                    objgrpfield.GroupID = defaultGrp.GroupID;
                    objgrpfield.AccountGUID = accountObj.AccountGUID;
                    var aa = CCGroupFieldRepository.SaveGroupField(objgrpfield);
                }
            }

            return RedirectToAction("ViewFields", "Folder", new { id = objFolderFields.FolderID });
        }

        [HttpPost]
        public ActionResult SelectedContactsFields(LayoutsViewModel objViewlayouts)
        {
            Account accountObj = (Account)Session["account"];
            User userObj = (User)Session["user"];
            CCFolder folderObj = (CCFolder)Session["folderDetail"];

            var objvalues = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(objViewlayouts.FieldValues);
            var objfield = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(objViewlayouts.FieldNames);

            var allfolderfields = CCFieldRepository.CCFolderFields.Where(aid => aid.FolderID == objViewlayouts.FolderID & aid.AccountGUID == accountObj.AccountGUID);
            var fieldvalues = CCFieldValueRepository.CCFieldValues.Where(cid => cid.ItemID == objViewlayouts.ContactID & cid.AccountGUID == accountObj.AccountGUID);
            var notes = "";
            int i = 0;
            foreach (var item in objvalues)
            {
                string itemValue = item;
                if (allfolderfields.Count() > 0 && fieldvalues.Count() > 0)
                {
                    string fieldname = "";
                    fieldname = objfield[i];
                    if (fieldname == "Start" || fieldname == "End")
                    {
                        string format = "yyyy-MM-dd HH:mm";
                        DateTime startEndTime = DateTimeOffset.Parse(item).UtcDateTime;
                        itemValue = startEndTime.ToString(format);
                    }
                    if(fieldname == "Notes")
                    {
                        notes = itemValue;
                    }
                    if (fieldname != "")
                    {
                        var fieldid = allfolderfields.FirstOrDefault(fname => fname.FieldName == fieldname).FieldID;
                        CCFieldValue objFieldValue = new CCFieldValue();
                        if (fieldvalues.FirstOrDefault(fid => fid.FieldID == fieldid) != null)
                        {
                            var valueid = fieldvalues.FirstOrDefault(fid => fid.FieldID == fieldid).ValueID;
                            objFieldValue.ValueID = valueid;
                        }
                        else
                        {
                            objFieldValue.LastUpdated = DateTime.UtcNow;
                            objFieldValue.ValueID = 0;
                            objFieldValue.FieldID = fieldid;
                            objFieldValue.ItemID = objViewlayouts.ContactID;
                        }
                        objFieldValue.Value = itemValue;
                        objFieldValue.AccountGUID = accountObj.AccountGUID;
                        CCFieldValueRepository.SaveFieldValues(objFieldValue);
                    }

                }

                i++;
            }
            long contactid = objViewlayouts.ContactID;
            if (contactid != 0)
            {
                AddDedupeViewModel dedupe = new AddDedupeViewModel();
                foreach (var field in allfolderfields.ToList())
                {
                    if (folderObj != null)
                    {
                        if (folderObj.Type == 1)
                        {
                            if (field.FieldCaption == "First Name") { dedupe.FirstName = fieldvalues.ToList().Find(fid => fid.FieldID == field.FieldID).Value; }
                            if (field.FieldCaption == "Middle Name") { dedupe.MiddleName = fieldvalues.ToList().Find(fid => fid.FieldID == field.FieldID).Value; }
                            if (field.FieldCaption == "Last Name") { dedupe.LastName = fieldvalues.ToList().Find(fid => fid.FieldID == field.FieldID).Value; }
                            if (field.FieldCaption == "Company") { dedupe.CompanyName = fieldvalues.ToList().Find(fid => fid.FieldID == field.FieldID).Value; }
                            if (field.FieldCaption == "Email Address") { dedupe.Email = fieldvalues.ToList().Find(fid => fid.FieldID == field.FieldID).Value; }
                        }
                        else if (folderObj.Type == 2)
                        {
                            if (field.FieldCaption == "Subject") { dedupe.Subject = fieldvalues.ToList().Find(fid => fid.FieldID == field.FieldID).Value; }
                            if (field.FieldCaption == "Start Time") { dedupe.StartDateTime = fieldvalues.ToList().Find(fid => fid.FieldID == field.FieldID).Value; }
                            if (field.FieldCaption == "End Time") { dedupe.EndDateTime = fieldvalues.ToList().Find(fid => fid.FieldID == field.FieldID).Value; }
                        }
                    }
                }

                CCItems contact = new CCItems();
                contact.ItemID = contactid;
                if (folderObj.Type == 1) { contact.DeDupeValue = dedupe.FirstName + "|" + dedupe.MiddleName + "|" + dedupe.LastName + "|" + dedupe.CompanyName + "|" + dedupe.Email; }
                else if (folderObj.Type == 2)
                { 
                    contact.DeDupeValue = dedupe.Subject + "|" + dedupe.StartDateTime + "|" + dedupe.EndDateTime;
                    contact.Notes = notes;
                    contact.TextBody = notes;
                }
                bool res = CCItemRepository.UpdateContact(contact);
            }

            return RedirectToAction("ViewLayout", "Folder", new { id = objViewlayouts.ContactID, fid = objViewlayouts.FolderID, pid = objViewlayouts.PageID });
        }

        public ActionResult ImportContacts(int id)
        {
            TempData["SelectedMenu"] = "Folder";
            TempData["SelectedSubMenuFolder"] = id;

            var folderName = CCFolderRepository.CCFolders.FirstOrDefault(fid => fid.FolderID == id).Name;
            ViewBag.foldername = folderName;

            return View(Convert.ToInt64(id));
        }

        [HttpPost]
        public ActionResult ImportContacts(long id)
        {
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];

            var folderName = CCFolderRepository.CCFolders.FirstOrDefault(fid => fid.FolderID == id & fid.AccountGUID == accountObj.AccountGUID).Name;
            ViewBag.foldername = folderName;

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                    string fileExtension = System.IO.Path.GetExtension(path);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        ViewBag.MessageExtention = "";

                        file.SaveAs(path);

                        ItemsImporter importcontact = new ItemsImporter(CCFieldRepository, CCFieldValueRepository, CCItemRepository);

                        LimitationsViewModel limitationsObj = (LimitationsViewModel)Session["limitations"];

                        long maxItemImportCount = limitationsObj.maxItemCountPerFolder - limitationsObj.folderList.Where(f => f.fold.FolderID == id).FirstOrDefault().itemCount;

                        long numberOfContacts = importcontact.ImportInputContacts(id, accountObj.AccountGUID, path, fileExtension, maxItemImportCount);

                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }

                        if (numberOfContacts == -2)
                        {
                            ViewBag.MessageItemImportExceed = "The number of contacts to be imported exceeds the Max Item per Folder Count";
                            //ViewBag.Message = "";
                            //ViewBag.MessagePass = "";
                        }
                        else if (numberOfContacts > 0)
                        {
                            ViewBag.MessagePass = "Import completed successfully, " + (numberOfContacts - 1) + " contacts were imported.";
                            //ViewBag.Message = "";
                            //ViewBag.MessageItemImportExceed = "";
                        }
                        else
                        {
                            ViewBag.Message = "No matching fields were found in the imported file. O contacts imported.";
                            //ViewBag.MessagePass = "";
                            //ViewBag.MessageItemImportExceed = "";
                        }

                    }

                    else
                    {
                        ViewBag.MessageExtention = "Please select the .xls or .xlsx file";
                        //ViewBag.MessagePass = "";
                        //ViewBag.MessageItemImportExceed = "";
                        //ViewBag.Message = "";
                    }

                }
            }

            LimitationsViewModel limitationsObjMain = (LimitationsViewModel)Session["limitations"];
            HelperFunctions HF = new HelperFunctions();
            limitationsObjMain = HF.updateAccountLimitations(accountObj);
            Session["limitations"] = limitationsObjMain;

            return View(id);
            //  return RedirectToAction("ImportContacts");
        }

        public ActionResult ImportBulkConnections(int fid)
        {

            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];
            var model = new ImportConnectionListModel();

            var folderName = CCFolderRepository.CCFolders.FirstOrDefault(f => f.FolderID == fid).Name;
            ViewBag.foldername = folderName;

            var FolderDetails = CCFolderRepository.CCFolders.Where(f => f.FolderID == fid).FirstOrDefault();

            model.ExistingCredentials = CCCredentialRepository.Credentials.Where(aguid => aguid.AccountGUID == accountObj.AccountGUID).ToList();
            model.selectedFolderID = FolderDetails.FolderID;

            if (FolderDetails.Type == 2)
            {
                model.ItemType = "Calendar";
            }
            else
            {
                model.ItemType = "Contacts";
            }

            return View(model);

        }

        [HttpPost]
        public ActionResult ImportBulkConnections(ImportConnectionListModel model)
        {

            long fid = model.selectedFolderID;

            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];
            long numberOfConnections = 0;

            var folderName = CCFolderRepository.CCFolders.FirstOrDefault(f => f.FolderID == fid & f.AccountGUID == accountObj.AccountGUID).Name;
            ViewBag.foldername = folderName;
            ConnectionImportListSummaryModel connSummary = new ConnectionImportListSummaryModel();

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                    string fileExtension = System.IO.Path.GetExtension(path);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        ViewBag.MessageExtention = "";

                        file.SaveAs(path);

                        LimitationsViewModel limitationsObj = (LimitationsViewModel)Session["limitations"];

                        long maxConnectionImportCount = limitationsObj.availableCconnectionCount;


                        ConnectionImporter importConnection = new ConnectionImporter(CCConnectinRepository, CCFolderRepository, CCCredentialRepository, CCFieldMappingsRepository);
                        connSummary = importConnection.ImportConnections(fid, accountObj.AccountGUID, path, fileExtension, model.SelectedCredentialID, model.SelectedImporsanationOrDelegation, model.SyncDirection, maxConnectionImportCount);

                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }

                        //if (numberOfConnections > 0)
                        //{
                        //    ViewBag.MessagePass = "Import completed successfully, " + (numberOfConnections - 1) + " contacts were imported.";
                        //    ViewBag.Message = "";
                        //}
                        //else
                        //{
                        //    ViewBag.Message = "No matching fields were found in the imported file. O contacts imported.";
                        //    ViewBag.MessagePass = "";
                        //}
                    }
                    else
                    {
                        ViewBag.MessageExtention = "Please select the .xls or .xlsx file";
                    }
                }
            }

            LimitationsViewModel limitationsObjMain = (LimitationsViewModel)Session["limitations"];
            HelperFunctions HF = new HelperFunctions();
            limitationsObjMain = HF.updateAccountLimitations(accountObj);
            Session["limitations"] = limitationsObjMain;

            Session["newConnections"] = connSummary;


            return RedirectToAction("ViewConnections", "Folder", new { id = fid });

        }



        public ActionResult DeleteContact(long id, long fid, long pid)
        {
            bool res = false;
            Account accountObj = (Account)Session["account"];

            var allFields = CCFieldValueRepository.CCFieldValues.Where(cid => cid.ItemID == id & cid.AccountGUID == accountObj.AccountGUID).ToList();


            if (allFields.Count > 0)
            {
                foreach (var cid in allFields)
                {
                    // res = CCFieldValueRepository.DeleteFieldValue(cid.ValueID);
                }
            }


            var contact = CCItemRepository.CCContacts.Where(cid => cid.ItemID == id & cid.AccountGUID == accountObj.AccountGUID).FirstOrDefault().ItemID;

            if (contact != null)
            {
                bool deletecontact = CCItemRepository.DeleteContact(contact);
            }

            var folderDetails = CCFolderRepository.CCFolders.FirstOrDefault(f => f.FolderID == fid);            

            LimitationsViewModel limitationsObj = (LimitationsViewModel)Session["limitations"];
            HelperFunctions HF = new HelperFunctions();
            limitationsObj = HF.updateAccountLimitations(accountObj);
            Session["limitations"] = limitationsObj;

            if (folderDetails.isOverFlow == true)
            { 
                if (limitationsObj.folderList.Where(f => f.fold.FolderID == id).FirstOrDefault().itemCount <= limitationsObj.maxItemCountPerFolder)
                {
                    folderDetails.isOverFlow = false;
                    folderDetails = CCFolderRepository.SaveFolder(folderDetails);
                }
            }
            return RedirectToAction("Items", "Folder", new { ID = fid });
        }

        public ActionResult Items(long ID)
        {
            Account accountObj = (Account)Session["account"];
            ItemsViewModel itemsViewModel = new ItemsViewModel();
            bool IsCrimeDiaryFields = false;

            TempData["SelectedMenu"] = "Folder";
            TempData["SelectedSubMenuFolder"] = ID;
            CCFolder folderDetail = new CCFolder();
            folderDetail = CCFolderRepository.FolderDetails(ID);

            if (folderDetail.Type == 1)
            {
                itemsViewModel.FolderDetails = folderDetail;
                Session["folderDetail"] = folderDetail;
                LimitationsViewModel limitationsObj = (LimitationsViewModel)Session["limitations"];
                itemsViewModel.limitationsObj = limitationsObj;

                int FolderItemCount = CCItemRepository.CCContacts.Where(i => i.FolderID == ID).Count();
                if ((FolderItemCount > limitationsObj.maxItemCountPerFolder) & (folderDetail.isOverFlow == false))
                {
                    folderDetail.isOverFlow = true;
                    CCFolderRepository.SaveFolder(folderDetail);
                }

                if ((FolderItemCount <= limitationsObj.maxItemCountPerFolder) & (folderDetail.isOverFlow == true))
                {
                    folderDetail.isOverFlow = false;
                    CCFolderRepository.SaveFolder(folderDetail);
                }

                //get folderfileds             
                List<CCFolderField> folderfields = new List<CCFolderField>();
                folderfields = CCFieldRepository.CCFolderFields.Where(id => id.FolderID == ID & id.AccountGUID == accountObj.AccountGUID).ToList();
                var isExisting = folderfields.Where(cap => cap.FieldName == "LawyerName" & cap.AccountGUID == accountObj.AccountGUID).FirstOrDefault();
                if (isExisting != null) { IsCrimeDiaryFields = true; }
                else { IsCrimeDiaryFields = false; }
                itemsViewModel.IsCrimeDiaryFields = IsCrimeDiaryFields;

                return View(itemsViewModel);
            }
            else
            {
                return RedirectToAction("viewAppointments", "Folder", new { ID = ID });
            }
        }

        public ActionResult ViewLayout(long id, long fid, long pid)
        {
            Account accountObj = (Account)Session["account"];
            string timeZone = (string)Session["timeZone"];
            TempData["SelectedMenu"] = "Folder";

            var itemInfor = CCItemRepository.CCContacts.Where(itmID => itmID.ItemID == id & itmID.AccountGUID == accountObj.AccountGUID).FirstOrDefault();
            if (!itemInfor.isDistGroup)
            {
                LayoutsViewModel ObjViewLayouts = new LayoutsViewModel();
                long noteGroupID = 0;
                bool isNoteShow = false;

                long layoutid = 1;

                var allLayouts = CCLayoutRepository.CCLayouts.Where(l => l.FolderID == fid & l.AccountGUID == accountObj.AccountGUID);
                var cou = allLayouts.Count();
                if (allLayouts.Count() >= 1)
                {
                    layoutid = allLayouts
                                 .FirstOrDefault().LayoutID;
                }

                var findNote = CCGroupRepository.CCGroups.FirstOrDefault(folder => folder.FolderID == fid && folder.GroupName == "Note" & folder.AccountGUID == accountObj.AccountGUID);

                if (findNote != null)
                {
                    noteGroupID = findNote.GroupID;
                    var isAddedNote = CCLayoutGroupRepository.CCLayoutGroups.FirstOrDefault(layout => layout.GroupID == noteGroupID && layout.LayoutID == layoutid & layout.AccountGUID == accountObj.AccountGUID);
                    if (isAddedNote != null)
                    {
                        isNoteShow = true;
                    }
                }

                ObjViewLayouts.LayoutDetails = CCGroupRepository.GetLayoutGroupsfieldsAndValues(layoutid, id, noteGroupID, timeZone);
                ObjViewLayouts.ContactID = id;
                ObjViewLayouts.FolderID = fid;
                ObjViewLayouts.FolderName = ((CCFolder)CCFolderRepository.CCFolders.Where(fold => fold.FolderID == ObjViewLayouts.FolderID).FirstOrDefault()).Name;
                ObjViewLayouts.PageID = pid;

                var isAvaiableNote = CCNoteRepository.CCNotes.FirstOrDefault(contid => contid.ContactID == id & contid.AccountGUID == accountObj.AccountGUID);
                if (isAvaiableNote != null) ObjViewLayouts.Note = isAvaiableNote.value;
                else ObjViewLayouts.Note = "";
                ObjViewLayouts.IsNoteShow = isNoteShow;
                return View(ObjViewLayouts);
            }
            else
            {
                return RedirectToAction("ViewDistributionList", "Folder", new { ID = id });
            }
        }

        public ActionResult ViewDistributionList(long ID)
        {
            Account accountObj = (Account)Session["account"];
            List<CCFieldValue> distListValues = new List<CCFieldValue>();
            distListValues = CCFieldValueRepository.CCFieldValues.Where(itmid => itmid.ItemID == ID & itmid.AccountGUID == accountObj.AccountGUID).ToList();
            return View(distListValues);
        }

        public ActionResult GroupMessage(long id)
        {
            return View(new CCGroup { FolderID = id });
        }

        public ActionResult AddGroup(long ID)
        {
            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageFolders";
            return View(new CCGroup { FolderID = ID });
        }

        public ActionResult EditGroup(long ID)
        {
            var userToEdit = CCGroupRepository.CCGroups.FirstOrDefault(u => u.FolderID == ID);
            if (userToEdit == null)
            {
                return View(new User());
            }
            else return View(userToEdit);
        }

        [HttpPost]
        public ActionResult EditGroup(CCGroup objGroup)
        {
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];

            if (ModelState.IsValid)
            {
                objGroup.AccountGUID = accountObj.AccountGUID;
                var resGroup = CCGroupRepository.SaveGroup(objGroup);
                if (resGroup != null)
                {
                    var isDefaultExist = CCLayoutRepository.CCLayouts.Where(fid => fid.FolderID == resGroup.FolderID & fid.LayoutName == "Default" & fid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();
                    if (isDefaultExist != null)
                    {
                        // get layout groups last order
                        var layoutGroupsOrder = CCLayoutGroupRepository.CCLayoutGroups.Where(gid => gid.LayoutID == isDefaultExist.LayoutID & gid.AccountGUID == accountObj.AccountGUID).OrderByDescending(grp => grp.GroupOrder).FirstOrDefault();

                        // add group on default layout                         
                        CCLayoutGroup objLayoutGroup = new CCLayoutGroup();
                        objLayoutGroup.GroupID = resGroup.GroupID;
                        objLayoutGroup.GroupOrder = layoutGroupsOrder.GroupOrder + 1;
                        objLayoutGroup.FolderID = resGroup.FolderID;
                        objLayoutGroup.LayoutID = isDefaultExist.LayoutID;
                        objLayoutGroup.AccountGUID = accountObj.AccountGUID;
                        var aa = CCLayoutGroupRepository.SavelayoutGroup(objLayoutGroup);
                    }
                }
                return RedirectToAction("ManageLayoutsAndGroups", "Folder", new { ID = objGroup.FolderID });
            }
            else
            {
                return View(objGroup);
            }
        }

        [HttpGet]
        public ActionResult DeleteGroup(long id, long fid)
        {
            DeleteGroupsAndGroupFields(id, fid);
            return RedirectToAction("ManageLayoutsAndGroups", "Folder", new { ID = fid });
        }

        private void DeleteGroupsAndGroupFields(long id, long fid)
        {

            bool resDelete = CCGroupRepository.DeleteGroup(id);
            if (resDelete)
            {
                CCGroupFieldRepository.DeleteFieldsByGroup(id);
                CCLayoutGroupRepository.DeleteLayoutGroupsByGroupID(id);
            }
        }

        public ActionResult SelectGroupFields(long ID)
        {
            GroupFieldsViewModel groupFields = new GroupFieldsViewModel();
            Account accountObj = (Account)Session["account"];
            //  get folder ID

            var groupDetails = CCGroupRepository.CCGroups.Where(u => u.GroupID == ID & u.AccountGUID == accountObj.AccountGUID).FirstOrDefault();
            long folderid = 0;
            if (groupDetails != null)
            {
                folderid = groupDetails.FolderID;
            }

            // get saved fields from groupfield table           
            var savedFields = CCGroupFieldRepository.CCGroupsFields.Where(grp => grp.GroupID == ID & grp.AccountGUID == accountObj.AccountGUID).ToList();

            // get field by Folder 
            User userObj = (User)Session["user"];
            var userToEdit = CCFieldRepository.CCFolderFields.Where(u => u.FolderID == folderid & u.AccountGUID == accountObj.AccountGUID).ToList();

            var fieldsName = userToEdit;
            List<string> savedFieldsName = new List<string>();

            foreach (var field in savedFields)
            {
                // add added fields name 
                var resu = fieldsName.FirstOrDefault(x => x.FieldID == field.FieldID);
                if (resu != null)
                {
                    savedFieldsName.Add(resu.FieldName);
                }

                // remove added fields
                userToEdit.RemoveAll(exist => exist.FieldID == field.FieldID);

            }

            var fieldsNameToAllFields = userToEdit.Select(f => f.FieldName).ToList();
            groupFields.AllFields = fieldsNameToAllFields;
            groupFields.SavedFields = savedFieldsName;
            groupFields.FolderID = folderid;
            groupFields.GroupID = ID;

            if (savedFieldsName.Count == 0) groupFields.IsSave = true;
            else groupFields.IsSave = false;

            // return View(fieldsNameToAllFields);   
            return View(groupFields);


        }

        [HttpPost]
        public ActionResult SelectGroupFields(string hid1, GroupFieldsViewModel grpfiled)
        {
            Account accountObj = (Account)Session["account"];
            var allFields = CCFieldRepository.CCFolderFields;
            var grpfields = allFields.Where(u => u.FolderID == grpfiled.FolderID & u.AccountGUID == accountObj.AccountGUID).ToList();

            CCGroupField objgrpfield = new CCGroupField();
            User userObj = (User)Session["user"];
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(hid1);
            int i = 1;

            if (grpfiled.IsSave == false)
            {
                CCGroupFieldRepository.DeleteFieldsByGroup(grpfiled.GroupID);
            }
            foreach (var item in obj)
            {
                var res = grpfields.Find(field => field.FieldName == item);
                if (res != null) objgrpfield.FieldID = res.FieldID;
                objgrpfield.FieldOrder = i++;
                objgrpfield.FolderID = grpfiled.FolderID;
                objgrpfield.GroupID = grpfiled.GroupID;
                objgrpfield.AccountGUID = accountObj.AccountGUID;
                var aa = CCGroupFieldRepository.SaveGroupField(objgrpfield);

            }

            return RedirectToAction("ManageLayoutsAndGroups", "Folder", new { ID = grpfiled.FolderID });
        }

        public ActionResult LayoutMessage(long id)
        {
            return View(new CCLayout { FolderID = id });
        }

        public ActionResult AddLayout(long ID)
        {
            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageFolders";
            return View(new CCLayout { FolderID = ID });
        }

        [HttpPost]
        public ActionResult EditLayout(CCLayout layoutObj)
        {
            Account accountObj = (Account)Session["account"];
            User userObj = (User)Session["user"];
            if (ModelState.IsValid)
            {
                layoutObj.AccountGUID = accountObj.AccountGUID;
                var user = CCLayoutRepository.Savelayout(layoutObj);
                return RedirectToAction("ManageLayoutsAndGroups", "Folder", new { ID = layoutObj.FolderID });
            }
            else
            {
                return View(layoutObj);
            }
        }

        [HttpGet]
        public ActionResult DeleteLayout(long id, long fid)
        {
            DeleteLayoutAndLayoutGroups(id, fid);
            return RedirectToAction("ManageLayoutsAndGroups", "Folder", new { ID = fid });
        }

        private void DeleteLayoutAndLayoutGroups(long id, long fid)
        {

            bool resDelete = CCLayoutRepository.Deletelayout(id);

            //delete layout group
            if (resDelete)
            {
                CCLayoutGroupRepository.DeleteLayoutGroupsByLayoutID(id);
            }
        }

        public ActionResult SelectLayoutGroups(long ID)
        {
            LayoutGroupsViewModel _vl = new LayoutGroupsViewModel();
            Account accountObj = (Account)Session["account"];
            //  get folder ID

            var alllayouts = CCLayoutRepository.CCLayouts;
            var layoutdetails = alllayouts.FirstOrDefault(u => u.LayoutID == ID & u.AccountGUID == accountObj.AccountGUID);
            long folderid = 0;
            if (layoutdetails != null)
            {
                folderid = layoutdetails.FolderID;
            }

            // get saved Groups from layoutgroup table           
            var allGroupsByLayout = CCLayoutGroupRepository.CCLayoutGroups;
            var savedGroups = allGroupsByLayout.Where(layout => layout.LayoutID == ID & layout.AccountGUID == accountObj.AccountGUID).ToList();


            // get Groups by Folder 
            var allGroups = CCGroupRepository.CCGroups;
            var userToEdit = allGroups.Where(u => u.FolderID == folderid & u.AccountGUID == accountObj.AccountGUID).ToList();

            var groupname = userToEdit;
            List<string> savedgroupssname = new List<string>();

            foreach (var group in savedGroups)
            {
                // add added fields name 
                var resu = groupname.FirstOrDefault(x => x.GroupID == group.GroupID);
                if (resu != null)
                {
                    savedgroupssname.Add(resu.GroupName);
                }

                // remove added fields
                userToEdit.RemoveAll(exist => exist.GroupID == group.GroupID);

            }

            var groupNamesToAllGroups = userToEdit.Select(f => f.GroupName).ToList();

            _vl.FolderID = folderid;
            _vl.LayoutID = ID;
            _vl.AllGroups = groupNamesToAllGroups;
            _vl.SavedGroups = savedgroupssname;

            if (savedgroupssname.Count == 0) _vl.IsSave = true;
            else _vl.IsSave = false;

            return View(_vl);
        }

        [HttpPost]
        public ActionResult SelectLayoutGroups(string groups, LayoutGroupsViewModel objLayoutField)
        {
            Account accountObj = (Account)Session["account"];
            // get Groups by Folder            
            var layoutGroup = CCGroupRepository.CCGroups
                            .Where(u => u.FolderID == objLayoutField.FolderID & u.AccountGUID == accountObj.AccountGUID).ToList();

            CCLayoutGroup objLayoutGroup = new CCLayoutGroup();
            User userObj = (User)Session["user"];
            var objgroups = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(groups);
            int i = 1;

            if (objLayoutField.IsSave == false)
            {
                CCLayoutGroupRepository.DeleteLayoutGroupsByLayoutID(objLayoutField.LayoutID);
            }
            foreach (var item in objgroups)
            {
                // objLayoutGroup.AccountID = userObj.AccountID;
                var res = layoutGroup.Find(field => field.GroupName == item);
                if (res != null) objLayoutGroup.GroupID = res.GroupID;
                objLayoutGroup.GroupOrder = i++;
                objLayoutGroup.FolderID = objLayoutField.FolderID;
                objLayoutGroup.LayoutID = objLayoutField.LayoutID;
                objLayoutGroup.AccountGUID = accountObj.AccountGUID;
                var aa = CCLayoutGroupRepository.SavelayoutGroup(objLayoutGroup);
            }
            return RedirectToAction("ManageLayoutsAndGroups", "Folder", new { ID = objLayoutField.FolderID });
        }

        public ActionResult ViewConnections(long id)
        {
            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageFolders";
            Account accountObj = (Account)Session["account"];
            LimitationsViewModel limitationsObj = (LimitationsViewModel)Session["limitations"];
            SubscriptionsViewModel subscriptionsView = new SubscriptionsViewModel();
            subscriptionsView.limitationsObj = limitationsObj;
            if (Session["newConnections"] != null)
            {
                subscriptionsView.conSummary = (ConnectionImportListSummaryModel) Session["newConnections"];

                Session["newConnections"] = null;
            }


            var folderDetails = CCFolderRepository.CCFolders.Where(fid => fid.FolderID == id & fid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();
            var subscriptions = CCConnectinRepository.CCSubscriptions.Where(fid => fid.FolderID == id & fid.AccountGUID == accountObj.AccountGUID);

            // get Credentials name
            List<string> credName = new List<string>();
            var allCredentials = CCCredentialRepository.Credentials.Where(aguid => aguid.AccountGUID == accountObj.AccountGUID).ToList();
            foreach (var conn in subscriptions)
            {
                var findCredential = allCredentials.Where(cid => cid.ID == conn.CredentialsID).FirstOrDefault();
                if (findCredential != null) { credName.Add(findCredential.Name); }
                else { credName.Add(""); }
            }
            subscriptionsView.CredentialName = credName;

            subscriptionsView.FolderName = folderDetails.Name;
            subscriptionsView.FolderType = folderDetails.Type;
            subscriptionsView.SubscriptionDetails = subscriptions;
            subscriptionsView.FolderID = id;

            //assign folder details 
            CCFolder folderDetail = new CCFolder();
            folderDetail = CCFolderRepository.FolderDetails(id);
            Session["folderDetail"] = folderDetail;

            return View(subscriptionsView);
        }

        public ActionResult EditConnection(long id, long sid)
        {
            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageFolders";
            SubscriptionsViewModel subscriptionsView = new SubscriptionsViewModel();
            Account accountObj = (Account)Session["account"];

            var folderName = CCFolderRepository.CCFolders.Where(foid => foid.FolderID == id & foid.AccountGUID == accountObj.AccountGUID).FirstOrDefault().Name;
            var subscription = CCConnectinRepository.CCSubscriptions.FirstOrDefault(ssid => ssid.ConnectionID == sid & ssid.AccountGUID == accountObj.AccountGUID);
            subscriptionsView.FolderID = id;
            subscriptionsView.Subscriptiondetail = subscription;
            subscriptionsView.FolderName = folderName;

            CCFolder folderObj = (CCFolder)Session["folderDetail"];
            if (folderObj != null) { subscriptionsView.FolderType = folderObj.Type; }

            if (subscription != null)
            {
                return View(subscriptionsView);
            }
            else
            {
                return RedirectToAction("Items", "Folder", new { ID = id });
            }

        }

        [HttpPost]
        public ActionResult EditConnection(SubscriptionsViewModel objSubscription)
        {
            if (ModelState.IsValid)
            {
                if (objSubscription.Subscriptiondetail.CategoryFilterUsed == false) { objSubscription.Subscriptiondetail.CategoryFilterValues = string.Empty; }
                var resp = CCConnectinRepository.UpdateSubscription(objSubscription.Subscriptiondetail);
                return RedirectToAction("ViewConnections", "Folder", new { id = objSubscription.FolderID });
            }
            else
            {
                return View(objSubscription);
            }
        }

        public ActionResult DeleteSubscriptions(long id)
        {
            Account accountObj = (Account)Session["account"];
            var selectedSubscription = CCConnectinRepository.CCSubscriptions.Where(sid => sid.ConnectionID == id & sid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();
            bool resp = false;
            if (selectedSubscription != null)
            {
                resp = CCConnectinRepository.DeleteSubscription(id);
            }

            LimitationsViewModel limitationsObj = (LimitationsViewModel)Session["limitations"];
            HelperFunctions HF = new HelperFunctions();
            limitationsObj = HF.updateAccountLimitations(accountObj);
            Session["limitations"] = limitationsObj;

            if(resp)
                return Json("success", JsonRequestBehavior.AllowGet);
            else
                return Json("fail", JsonRequestBehavior.AllowGet);


            //return RedirectToAction("ViewConnections", "Folder", new { id = selectedSubscription.FolderID });
        }

        public ActionResult LogintoExchange(string src = "", long fid = 1)
        {
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];
            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageFolders";
            var model = new ExLogOnViewModel();
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

        public ActionResult SelectFolder(string src = "", long primid = 1, long creid = 1)
        {
            int type = CCFolderRepository.CCFolders.FirstOrDefault(fid => fid.FolderID == primid).Type;
            Session["ExistingCreError"] = "";

            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageFolders";
            ExchangeService ex = Session["srv"] as ExchangeService;
            bool addingPrimary = false;
            long primarySourceID = 0;
            long credentialsID = 0;
            try { primarySourceID = Convert.ToInt64(primid); }
            catch { }

            try { credentialsID = Convert.ToInt64(creid); }
            catch { }

            if (src == "prim")
            {
                addingPrimary = true;
            }

            bool addTopFolder = false;
            if (ex != null)
            {
                string accessTypes = "0";
                string secondaryAccount = Session["secondaryAccount"] as string;
                if (Session["accessType"] != null) { accessTypes = Session["accessType"].ToString(); }

                Folder f = null;
                Folder publicFolder = null;
                try
                {
                    if (accessTypes == "1")
                    {
                        if (type == 1)
                        {
                            f = Folder.Bind(ex, new FolderId(WellKnownFolderName.Contacts, secondaryAccount));
                            Session["differentMailboxError"] = "No";
                        }
                        else
                        {
                            f = Folder.Bind(ex, new FolderId(WellKnownFolderName.Calendar, secondaryAccount));
                            Session["differentMailboxError"] = "No";
                        }

                        addTopFolder = true;

                    }
                    else if (accessTypes == "2")
                    {
                        ex.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, secondaryAccount);
                        if (type == 1)
                        {
                            f = Folder.Bind(ex, new FolderId(WellKnownFolderName.Contacts, secondaryAccount));
                            Session["differentMailboxError"] = "No";
                        }
                        else
                        {
                            f = Folder.Bind(ex, new FolderId(WellKnownFolderName.Calendar, secondaryAccount));
                            Session["differentMailboxError"] = "No";
                        }

                        addTopFolder = true;
                    }
                    else
                    {
                        f = Folder.Bind(ex, WellKnownFolderName.MsgFolderRoot);
                        try
                        {
                            publicFolder = Folder.Bind(ex, WellKnownFolderName.PublicFoldersRoot);
                        }
                        catch (Exception)
                        {
                            publicFolder = null;
                        }
                        Session["differentMailboxError"] = "No";
                        addTopFolder = false;
                    }
                }
                catch (Exception e)
                {
                    string exMessage = e.Message;
                    if (exMessage == "The request failed. The remote server returned an error: (404) Not Found.") { Session["ExistingCreError"] = "Error: Server returned 'Unauthorized'."; }
                    else if (exMessage == "The request failed. The remote server returned an error: (401) Unauthorized.") { Session["ExistingCreError"] = "Error: Server returned 'Unauthorized'."; }
                    else { Session["differentMailboxError"] = "Yes"; }
                    return RedirectToAction("LogintoExchange", "Folder", new { src = "sec", fid = primid });

                }

                //var f = Folder.Bind(ex, WellKnownFolderName.MsgFolderRoot);
                List<TreeViewFolder> FolderLists = new List<TreeViewFolder>();
                TreeViewFolder mailBox = new TreeViewFolder();
                TreeViewFolder publicBox = new TreeViewFolder();

                if (f != null)
                {
                    var tree = LoadSubFolders(f, new TreeViewFolder(), addTopFolder);

                    mailBox.Name = "*Mail Folders";
                    mailBox.FolderClass = "All";

                    foreach (var it in tree.ChildFolders)
                    {
                        mailBox.ChildFolders.Add(it);
                    }
                    FolderLists.Add(mailBox);
                }

                if (publicFolder != null)
                {
                    var treepublic = LoadSubFolders(publicFolder, new TreeViewFolder(), addTopFolder);

                    publicBox.Name = "*Public Folders";
                    publicBox.FolderClass = "All";

                    foreach (var it in treepublic.ChildFolders)
                    {
                        publicBox.ChildFolders.Add(it);
                    }

                    FolderLists.Add(publicBox);
                }

                var model = new SelectFolderViewModel();
                // model.FolderList = tree.ChildFolders.ToList();
                model.FolderList = FolderLists;
                model.AddingPrimary = addingPrimary;
                model.PrimarySourceId = primarySourceID;
                model.CredentialID = credentialsID;
                model.UniqueId = f.Id.UniqueId;
                model.SelectedFolderOwnerInfo = Session["srvEmail"] as string;
                model.FolderType = type;
                return View(model);
            }
            else
            {
                var model = new SelectFolderViewModel();
                model.FolderList = new List<TreeViewFolder>();
                model.AddingPrimary = addingPrimary;
                model.PrimarySourceId = primarySourceID;
                model.CredentialID = credentialsID;
                model.FolderType = type;
                return View(model);
            }
        }

        private TreeViewFolder LoadSubFolders(Folder f, TreeViewFolder subtree, bool addTopFolder)
        {

            var view = new FolderView(100);
            view.Traversal = FolderTraversal.Shallow;
            var findFolderResults = f.FindFolders(view);

            string typeClass = "Contact";

            if ((((CCFolder)Session["folderDetail"])) != null)
            {
                if ((((CCFolder)Session["folderDetail"]).Type) == 1) { typeClass = "Contact"; }
                else { typeClass = "Appointment"; }
            }


            if (addTopFolder)
            {
                var childNode = new TreeViewFolder();
                childNode.Name = f.DisplayName;
                childNode.Id = f.Id.UniqueId;
                var fclass = f.FolderClass;
                if (fclass == null) fclass = "All";
                if (fclass == "IPF.Contact") fclass = "Contact";
                else if (fclass == "IPF.Appointment") fclass = "Appointment";
                else if (fclass == "IPF.Task") fclass = "Task";
                else fclass = "All";
                childNode.FolderClass = fclass;
                subtree.ChildFolders.Add(childNode);
            }
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
                    if (typeClass == fclass)
                    {
                        subtree.ChildFolders.Add(LoadSubFolders(item, childNode, false));
                    }
                    else if (fclass == "All")
                    {
                        subtree.ChildFolders.Add(LoadSubFolders(item, childNode, false));
                    }
                }
                else
                {
                    if (typeClass == fclass)
                    {
                        subtree.ChildFolders.Add(childNode);
                    }
                }
            }
            return subtree;
        }

        [HttpPost]
        public ActionResult SelectFolder(SelectFolderViewModel model)
        {
            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageFolders";
            User userObj = (User)Session["user"];
            if (!string.IsNullOrEmpty(model.SelectedFolderId))
            {

                return RedirectToAction("SelectFolderoptions", "Folder", model);
            }
            return View(new List<TreeViewFolder>());
        }

        public ActionResult SelectFolderoptions(SelectFolderViewModel objfolder)
        {
            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageFolders";
            CCFolder folderObj = (CCFolder)Session["folderDetail"];
            if (folderObj != null) { objfolder.FolderType = folderObj.Type; }

            return View(objfolder);
        }

        [HttpPost]
        public ActionResult SelectFolderoptionss(SelectFolderViewModel model)
        {
            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageFolders";
            Account accountObj = (Account)Session["account"];
            int type = 1;
            CCFolder folderDetails = null;
            folderDetails = CCFolderRepository.FolderDetails(model.PrimarySourceId);
            if (folderDetails != null) { type = folderDetails.Type; }
            else { type = 1; }

            User userObj = (User)Session["user"];

            var connection = new CorporateContacts.Domain.Entities.CCConnection();
            connection.FolderID = model.PrimarySourceId;
            connection.FolderName = model.SelectedFolderName;
            connection.Owner = model.SelectedFolderOwnerInfo;
            if (type == 1) { connection.Type = "Contact"; }
            else { connection.Type = "Calendar"; }
            connection.CredentialsID = model.CredentialID;
            connection.AllowAdditions = false;
            connection.IgnoreExisting = model.IgnoreExisting;
            connection.SyncDirection = model.SyncDirection;
            connection.CategoryFilterUsed = model.CategoryFilterUsed;
            connection.CopyPhotos = model.CopyPhotos;
            connection.TurnOffReminders = model.TurnOffReminders;
            connection.SourceID = model.SelectedFolderId;
            connection.Frequency = 1440;
            connection.IsRunning = false;
            connection.IsPaused = false;
            connection.SecondaryAccount = Session["secondaryAccount"] as string;
            string accessTypes = string.Empty;
            if (Session["accessType"] != null) { accessTypes = Session["accessType"].ToString(); }
            if (string.IsNullOrEmpty(accessTypes)) { connection.AccessType = 0; }
            else { connection.AccessType = Int32.Parse(accessTypes); }
            string format = "yyyy-MM-dd HH:mm";
            connection.LastSyncTime = ((DateTime)(System.Data.SqlTypes.SqlDateTime.MinValue)).ToString(format);
            if (model.CategoryFilterUsed == false) { connection.CategoryFilterValues = string.Empty; }
            else { connection.CategoryFilterValues = model.CategoryFilterValues; }
            if (model.TagAllSubject == true) { connection.SubjectTag = model.SubjectTag; }
            else { connection.SubjectTag = string.Empty; }
            connection.tagSubject = model.TagAllSubject;
            connection.AccountGUID = accountObj.AccountGUID;
            connection = CCConnectinRepository.SaveSubscription(connection);

            if (connection != null)
            {
                var foldertag = new CorporateContacts.Domain.Entities.CCConnection();
                var tagname = connection.FolderName + "[" + connection.ConnectionID + "]";
                foldertag.Tag = tagname;
                foldertag.ConnectionID = connection.ConnectionID;

                CCConnectinRepository.SaveSubscriptionTag(foldertag);
            }

            var res = CCFieldMappingsRepository.SaveAllMappingFields(connection.FolderID, connection.ConnectionID, accountObj.AccountGUID);

            LimitationsViewModel limitationsObj = (LimitationsViewModel)Session["limitations"];
            HelperFunctions HF = new HelperFunctions();
            limitationsObj = HF.updateAccountLimitations(accountObj);
            Session["limitations"] = limitationsObj;

            return RedirectToAction("ViewConnections", "Folder", new { id = connection.FolderID });
        }

        [HttpPost]
        public ActionResult LogintoExchange(ExLogOnViewModel model, bool hosted = false)
        {
            EWSCode ewscode = new EWSCode();
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];
            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageFolders";
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
                    ViewBag.msucess = "Connection Successful";
                    ViewBag.mfail = "";
                    ViewBag.tmessage = "sucess";
                    ViewBag.pass = Encryption.DecryptStringAES(model.Credentials.Password, rand);
                    model.ServerVer = cc.version.ToString();
                    Session["secondaryAccount"] = model.SecondaryAccount;
                    Session["accessType"] = model.AccessType;
                    string accessTypes = Session["accessType"].ToString();
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
                    ViewBag.msucess = "Connection Successful";
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
                    ViewBag.msucessin = "Connection Successful";
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
                    ViewBag.msucessin = "Connection Successful";
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
            else
            {

                if (model.SecondaryAccountc != null)
                {
                    Session["secondaryAccount"] = model.SecondaryAccountc;
                    if (model.AccessTypec != null) { Session["accessType"] = model.AccessTypec; }
                }

                if (model.SecondaryAccount != null)
                {
                    Session["secondaryAccount"] = model.SecondaryAccount;
                    if (model.AccessType != null) { Session["accessType"] = model.AccessType; }
                }

                if (model.SecondaryAccountIn != null)
                {
                    Session["secondaryAccount"] = model.SecondaryAccountIn;
                    if (model.AccessTypeIn != null) { Session["accessType"] = model.AccessTypeIn; }
                }

                if (model.SecondaryAccountc == null && model.SecondaryAccount == null && model.SecondaryAccountIn == null) { Session["accessType"] = 0; Session["secondaryAccount"] = ""; }


                ExchangeService srv = null;
                if (model.SelectedCredentialID != 0 || model.CreatedCredentialID != 0 || model.CreatedCredentialIDIn != 0)
                {
                    long selectedCreadintialID = 0;
                    if (model.SelectedCredentialID != 0) { selectedCreadintialID = model.SelectedCredentialID; }
                    else if (model.CreatedCredentialID != 0) { selectedCreadintialID = model.CreatedCredentialID; }
                    else if (model.CreatedCredentialIDIn != 0) { selectedCreadintialID = model.CreatedCredentialIDIn; }

                    var creds = CCCredentialRepository.Credentials.FirstOrDefault(x => x.ID == selectedCreadintialID);

                    string depassword = Encryption.DecryptStringAES(creds.Password, rand);
                    if (creds.IsHostedExchange)
                    {
                        srv = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                        if (creds.EmailAddress != null) { srv.Credentials = new WebCredentials(creds.EmailAddress, depassword); }
                        else { srv.Credentials = new WebCredentials(creds.UserName, depassword); }
                        srv.Url = new Uri(creds.URL);
                    }
                    else
                    {
                        if (creds.ServerVersion == "2007SP1") srv = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                        else if (creds.ServerVersion == "2010") srv = new ExchangeService(ExchangeVersion.Exchange2010);
                        else if (creds.ServerVersion == "2010SP1") srv = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
                        else if (creds.ServerVersion == "Exchange2010_SP1") srv = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
                        else if (creds.ServerVersion == "Exchange2010_SP2") srv = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
                        else srv = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                        if (creds.Domain != null) { srv.Credentials = new WebCredentials(creds.UserName, depassword, creds.Domain); }
                        else if (creds.UserName != null) { srv.Credentials = new WebCredentials(creds.UserName, depassword); }
                        else { srv.Credentials = new WebCredentials(creds.EmailAddress, depassword); }
                        srv.Url = new Uri(creds.URL);
                    }
                    Session["srv"] = srv;
                    if (creds.EmailAddress != null) { Session["srvEmail"] = creds.EmailAddress; }
                    else { Session["srvEmail"] = creds.UserName; }
                    credentialID = selectedCreadintialID;
                }
                return Redirect(model.ReturnUrl + "&creid=" + credentialID);
            }
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
                    ViewBag.msucess = "Connection Successful and Saved";
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
                    ViewBag.msucess = "Connection Successful and Saved";
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
                    ViewBag.msucessin = "Connection Successful and Saved";
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
                    ViewBag.msucessin = "Connection Successful and Saved";
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

        public ActionResult SelectMappingFields(long ID, long sid)
        {
            Account accountObj = (Account)Session["account"];
            MappingFieldsViewModel mappingFields = new MappingFieldsViewModel();


            var mapFields = CCFieldRepository.CCFolderFields
                            .Where(fid => fid.FolderID == ID & fid.AccountGUID == accountObj.AccountGUID).ToList();

            var userToedit = mapFields;

            var savedFields = CCFieldMappingsRepository.CCFieldsMapping
                             .Where(ssid => ssid.ConnectionID == sid & ssid.AccountGUID == accountObj.AccountGUID).ToList();

            List<string> savedfieldsname = new List<string>();

            foreach (var sav in savedFields)
            {
                // add added fields name 
                var resfield = mapFields.FirstOrDefault(fid => fid.FieldID == sav.MappedFieldID);
                if (resfield != null)
                {
                    savedfieldsname.Add(resfield.FieldCaption);
                }

                // remove added fields
                userToedit.RemoveAll(exist => exist.FieldID == sav.MappedFieldID);
            }


            mappingFields.AllMappingFields = userToedit.Select(fname => fname.FieldCaption).ToList();
            mappingFields.FolderID = ID;
            mappingFields.SubscriptionID = sid;
            mappingFields.SavedMappingFields = savedfieldsname;
            return View(mappingFields);
        }

        [HttpPost]
        public ActionResult SelectMappingFields(string fields, MappingFieldsViewModel objMappingFileds)
        {
            Account accountObj = (Account)Session["account"];
            CCFieldMapping fieldMapping = new CCFieldMapping();
            var objFields = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(fields);
            var mappingfields = CCFieldRepository.CCFolderFields.Where(fid => fid.FolderID == objMappingFileds.FolderID).ToList();

            var savedfield = CCFieldMappingsRepository.CCFieldsMapping
                           .Where(sid => sid.ConnectionID == objMappingFileds.SubscriptionID).ToList();

            if (savedfield.Count > 0)
            {
                CCFieldMappingsRepository.DeleteMappingFieldBySubscriptionID(objMappingFileds.SubscriptionID);

            }

            foreach (var mfield in objFields)
            {
                var mapfield = mappingfields.FirstOrDefault(fname => fname.FieldCaption == mfield);

                if (mapfield != null)
                {
                    fieldMapping.ConnectionID = objMappingFileds.SubscriptionID;
                    fieldMapping.FieldName = FieldsConfigHelper.GetRealName(mapfield.FieldName);
                    fieldMapping.Caption = mapfield.FieldCaption;
                    fieldMapping.MappedFieldID = mapfield.FieldID;
                    fieldMapping.AccountGUID = accountObj.AccountGUID;

                    var res = CCFieldMappingsRepository.SaveFieldMapping(fieldMapping);
                }
            }

            return RedirectToAction("ViewConnections", "Folder", new { id = objMappingFileds.FolderID });
        }

        public ActionResult SaveNote(string note, string cid)
        {
            Account accountObj = (Account)Session["account"];
            long contactID = Convert.ToInt64(cid);
            long noteID = 0;

            var isAvaiableNote = CCNoteRepository.CCNotes.FirstOrDefault(contid => contid.ContactID == contactID & contid.AccountGUID == accountObj.AccountGUID);

            if (isAvaiableNote != null)
            {
                noteID = isAvaiableNote.NotesID;
            }

            CCNote objNote = new CCNote();
            objNote.value = note.Trim();
            objNote.ContactID = contactID;
            objNote.NotesID = noteID;
            var res = CCNoteRepository.SaveNote(objNote);

            if (res != null)
            {
                return Json("sucess", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult AddContact(long id, int state)
        {
            int type = 1;
            CCFolder folderDetails = null;

            AddContactViewModel folderFields = new AddContactViewModel();
            folderFields.FolderFields = CCFieldRepository.CCFolderFields.Where(fid => fid.FolderID == id).ToList();
            folderFields.FolderID = id;
            

            folderDetails = CCFolderRepository.FolderDetails(id);
            if (folderDetails.isCrimeDiary == true)
                folderFields.isCrimeDiary = true;
            else
                folderFields.isCrimeDiary = false;

            if (folderDetails != null) { type = folderDetails.Type; }
            else { type = 1; }
            folderFields.FolderType = type;

            if (state == 1) { ViewBag.Message = "Failed to import contact"; }
            else { ViewBag.Message = ""; }
            return View(folderFields);
        }

        [HttpPost]
        public ActionResult AddContact(AddContactViewModel objContact)
        {
            Account accountObj = (Account)Session["account"];
            string timeZone = (string)Session["timeZone"];
            int type = 1;
            ItemsImporter importcontact = new ItemsImporter(CCFieldRepository, CCFieldValueRepository, CCItemRepository);
            CCFolder folderDetails = null;
            folderDetails = CCFolderRepository.FolderDetails(objContact.FolderID);
            if (folderDetails != null) { type = folderDetails.Type; }
            else { type = 1; }
            bool res = importcontact.ImportSingleContact(objContact, type, accountObj.AccountGUID, timeZone);

            LimitationsViewModel limitationsObj = (LimitationsViewModel)Session["limitations"];
            HelperFunctions HF = new HelperFunctions();
            limitationsObj = HF.updateAccountLimitations(accountObj);
            Session["limitations"] = limitationsObj;


            if (res) return RedirectToAction("Items", "Folder", new { id = objContact.FolderID });
            else { return RedirectToAction("AddContact", "Folder", new { id = objContact.FolderID, state = 1 }); }
        }

        public ActionResult ItemList(int pageNumber, int recordsPerPage, string sortColumn, string sortOrder, string sortBy, long folderID, int type, string searchField, string searchValue)
        {
            Account accountObj = (Account)Session["account"];
            string timeZone = (string)Session["timeZone"];
            if (string.IsNullOrEmpty(timeZone)) { timeZone = "UTC"; }
            var jqGridObject = CCItemRepository.GetJQModel(pageNumber, recordsPerPage, sortColumn, sortOrder, sortBy, folderID, type, searchField, searchValue, timeZone, accountObj.AccountGUID);
            return Json(jqGridObject, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ItemView(long id)
        {
            CCFolder folderDetail = new CCFolder();
            folderDetail = CCFolderRepository.FolderDetails(id);
            return View(folderDetail);
        }

        [HttpPost]
        public ActionResult AddCrimeDiary(ExLogOnViewModel model, bool hosted = false)
        {
            Account accountObj = (Account)Session["account"];
            DateTime? d = null;
            var folderID = ((CCFolder)Session["folderDetail"]).FolderID;
            var folderNmae = ((CCFolder)Session["folderDetail"]).Name;
            string constring = string.Empty;
            var hostUrl = System.Web.HttpContext.Current.Request.Url.Host;
            if (hostUrl == "secure.corporate-contacts.com") { constring = ConfigurationManager.ConnectionStrings["TuckersConnectionDB"].ConnectionString; }
            else if (hostUrl == "staging.corporate-contacts.com") { constring = ConfigurationManager.ConnectionStrings["TuckersConnectionDB"].ConnectionString; }
            else { constring = ConfigurationManager.ConnectionStrings["TuckersConnectionTestDB"].ConnectionString; }

            CCConnection objConnection = new CCConnection();
            objConnection = model.Connection;
            objConnection.Type = "CrimeDiary";
            objConnection.FolderID = folderID;
            objConnection.AllowAdditions = false;
            objConnection.IgnoreExisting = false;
            objConnection.CategoryFilterUsed = false;
            objConnection.CopyPhotos = false;
            objConnection.TurnOffReminders = false;
            objConnection.CredentialsID = 11111;
            objConnection.Owner = "owner";
            objConnection.IsRunning = false;
            objConnection.LastSyncTime = "1900-01-01 00:00";
            objConnection.Frequency = 1440;
            objConnection.FolderName = folderNmae;
            objConnection.SourceID = constring;
            objConnection.AccessType = 0;
            objConnection.SecondaryAccount = "";
            objConnection.AccountGUID = accountObj.AccountGUID;

            if (ModelState.IsValid)
            {
                var resp = CCConnectinRepository.SaveSubscription(objConnection);
                return RedirectToAction("ViewConnections", "Folder", new { id = folderID });
            }
            else
            {
                return View(model);
            }


        }

        public ActionResult ValidateToken(string tokenId)
        {
            bool isMatch = false;
            DBHelper dbObj = new DBHelper();
            String query = string.Empty;
            string constring = string.Empty;

            var hostUrl = System.Web.HttpContext.Current.Request.Url.Host;
            if (hostUrl == "secure.corporate-contacts.com") { constring = ConfigurationManager.ConnectionStrings["TuckersConnectionDB"].ConnectionString; }
            else if (hostUrl == "staging.corporate-contacts.com") { constring = ConfigurationManager.ConnectionStrings["TuckersConnectionDB"].ConnectionString; }
            else { constring = ConfigurationManager.ConnectionStrings["TuckersConnectionTestDB"].ConnectionString; }

            //query = "SELECT * FROM tblGroups WHERE SyncToken= '" + tokenId + "'";
            query = "SELECT * FROM tblGroups WHERE UserSignupCode= '" + tokenId + "'";

            DataSet ds = new DBHelper().GetDataSet(query, constring);

            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count == 0) isMatch = false;
                else isMatch = true;
            }

            if (isMatch == true)
            {
                return Json("sucess", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult DeleteFolder(long ID)
        {
            Account accountObj = (Account)Session["account"];

            // find all layouts          
            var layouts = CCLayoutRepository.CCLayouts.Where(fid => fid.FolderID == ID & fid.AccountGUID == accountObj.AccountGUID).ToList();
            foreach (var layout in layouts)
            {
                DeleteLayoutAndLayoutGroups(layout.LayoutID, ID);
            }

            //find all groups            
            var groups = CCGroupRepository.CCGroups.Where(gid => gid.FolderID == ID & gid.AccountGUID == accountObj.AccountGUID).ToList();
            foreach (var group in groups)
            {
                DeleteGroupsAndGroupFields(group.GroupID, ID);
            }

            //Delete all history items
            var fieldList = CCFieldRepository.CCFolderFields.Where(field => field.FolderID == ID).ToList();
            foreach(var field in fieldList)
            {
                 CCHistoryLogRepository.DeleteHistoryLogByFieldID(field.FieldID);   
            }
            
            //Delete all field values
            List<CCItems> itemList = CCItemRepository.CCContacts.Where(item=>item.FolderID == ID).ToList();

            foreach (var item in itemList)
            {
                CCFieldValueRepository.DeleteFieldValueByITemID(item.ItemID);
            }

            //Delete all field mappings
            List<CCConnection> connList = CCConnectinRepository.CCSubscriptions.Where(con => con.FolderID == ID).ToList();

            foreach (var conn in connList)
            {
                CCFieldMappingsRepository.DeleteMappingFieldBySubscriptionID(conn.ConnectionID);
            }

            //Delete all items and field value            
            CCItemRepository.DeleteItems(ID);

            //Delete all folder field            
            CCFieldRepository.DeleteFolderFields(ID);

            //Delete connection and mapping field        
            CCConnectinRepository.DeleteConnections(ID);

            //Delete folder         
            CCFolderRepository.DeleteFolder(ID);

            //set new folder session 
            var folders = CCFolderRepository.CCFolders.Where(aguid => aguid.AccountGUID == accountObj.AccountGUID).ToList();
            Session["folderss"] = folders;

            return RedirectToAction("ManageFolders", "Folder");
        }

        public ActionResult ManageFolders()
        {
            TempData["SelectedMenu"] = "Manage";
            TempData["SelectedSubMenu"] = "ManageFolders";
            Account accountObj = (Account)Session["account"];

            ManageFoldersViewModel manageFolders = new ManageFoldersViewModel();
            List<CCFolder> folders = CCFolderRepository.CCFolders.Where(guid => guid.AccountGUID == accountObj.AccountGUID).ToList();
            List<Credential> credentials = CCCredentialRepository.Credentials.Where(guid => guid.AccountGUID == accountObj.AccountGUID).ToList();

            manageFolders.Credentials = credentials;
            manageFolders.Folders = folders;

            return View(manageFolders);
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

        public ActionResult GetChangeHistoryForItem(long ID)
        {
            List<CCHistoryLog> HistLog = CCHistoryLogRepository.CCHistoryLogs.Where(hLog => hLog.ItemID == ID).ToList();

            string historyLogTableContent = "";
            foreach (CCHistoryLog log in HistLog)
            {

                if (log.Action == "Delete")
                    historyLogTableContent = historyLogTableContent + "<tr style=\"color:#FF0000;\">";
                else
                    historyLogTableContent = historyLogTableContent + "<tr>";

                if (log.Action == "Insert")
                    historyLogTableContent = historyLogTableContent + "<td>" + log.Date.ToString() + "<span style=\"color:#FF0000;\">*</span></td>";
                else
                    historyLogTableContent = historyLogTableContent + "<td>" + log.Date.ToString() + "</td>";

                if (log.Action != "Delete" & log.Action != "Delete-Revert")
                {
                    CCFolderField FF = CCFieldRepository.CCFolderFields.Where(ff => ff.FieldID == log.FieldID).FirstOrDefault();
                    historyLogTableContent = historyLogTableContent + "<td>" + FF.FieldName + "</td>";
                    historyLogTableContent = historyLogTableContent + "<td>" + log.OldValue + "</td>";
                    historyLogTableContent = historyLogTableContent + "<td>" + log.NewValue + "</td>";
                }
                else
                {
                    historyLogTableContent = historyLogTableContent + "<td>-</td>";
                    historyLogTableContent = historyLogTableContent + "<td>-</td>";
                    historyLogTableContent = historyLogTableContent + "<td>-</td>";
                }

                if (log.Action != "Insert")
                {
                    if (log.Action != "Delete")
                        if (log.Action != "Delete-Revert")
                            historyLogTableContent = historyLogTableContent + "<td><a style=\"color:#478fca;\" onClick=\"ConfirmRevertChange('" + log.ID + "',2,'update');\" href=\"#\">Revert Change</a></td>";
                        else
                            historyLogTableContent = historyLogTableContent + "<td disabled></td>";
                    else
                        historyLogTableContent = historyLogTableContent + "<td><a style=\"color:#478fca;\" onClick=\"ConfirmRevertChange('" + log.ID + "',2,'delete');\" href=\"#\">Revert Change</a></td>";
                }
                else
                    historyLogTableContent = historyLogTableContent + "<td disabled></td>";
                historyLogTableContent = historyLogTableContent + "</tr>";
            }
            return Json(historyLogTableContent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChangeHistoryForFolder(long ID)
        {
            List<CCItems> itemList = CCItemRepository.CCContacts.Where(il => il.FolderID == ID).ToList();

            List<CCHistoryLog> HistLog = new List<CCHistoryLog>();

            foreach(CCItems item in itemList)
            {
                HistLog.AddRange(CCHistoryLogRepository.CCHistoryLogs.Where(hl => hl.ItemID == item.ItemID).ToList());
            }

            string historyLogTableContent = "";
            foreach (CCHistoryLog log in HistLog)
            {

                if (log.Action == "Delete")
                    historyLogTableContent = historyLogTableContent + "<tr style=\"color:#FF0000;\">";
                else if  (log.Action == "Delete-Revert")
                    historyLogTableContent = historyLogTableContent + "<tr style=\"color:#33CC33;\">";
                else
                    historyLogTableContent = historyLogTableContent + "<tr>";

                if(log.Action=="Insert")
                    historyLogTableContent = historyLogTableContent + "<td>" + log.Date.ToString() + "<span style=\"color:#FF0000;\">*</span></td>";
                else
                    historyLogTableContent = historyLogTableContent + "<td>" + log.Date.ToString() + "</td>";
                
                CCItems item = CCItemRepository.CCContacts.Where(i => i.ItemID == log.ItemID).FirstOrDefault();
                historyLogTableContent = historyLogTableContent + "<td><a style=\"cursor:default;\" data-rel=\"tooltip\" title=\"" + getTitleForItem(item) + "\">" + log.ItemID + "</a></td>";
                if (log.Action != "Delete" & log.Action != "Delete-Revert")
                {                    
                    CCFolderField FF = CCFieldRepository.CCFolderFields.Where(ff => ff.FieldID == log.FieldID).FirstOrDefault();
                    historyLogTableContent = historyLogTableContent + "<td>" + FF.FieldName + "</td>";
                    historyLogTableContent = historyLogTableContent + "<td>" + log.OldValue + "</td>";
                    historyLogTableContent = historyLogTableContent + "<td>" + log.NewValue + "</td>";
                }
                else
                {
                    historyLogTableContent = historyLogTableContent + "<td>-</td>";
                    historyLogTableContent = historyLogTableContent + "<td>-</td>";
                    historyLogTableContent = historyLogTableContent + "<td>-</td>";
                }


                if (log.Action != "Insert")
                {
                    if (log.Action != "Delete")
                        if (log.Action != "Delete-Revert")
                            historyLogTableContent = historyLogTableContent + "<td><a style=\"color:#478fca;\" onClick=\"ConfirmRevertChange('" + log.ID + "',1,'update');\" href=\"#\">Revert Change</a></td>";
                        else
                            historyLogTableContent = historyLogTableContent + "<td disabled></td>";
                    else                        
                        historyLogTableContent = historyLogTableContent + "<td><a style=\"color:#478fca;\" onClick=\"ConfirmRevertChange('" + log.ID + "',1,'delete');\" href=\"#\">Revert Change</a></td>";
                }
                else
                    historyLogTableContent = historyLogTableContent + "<td disabled></td>";
                historyLogTableContent = historyLogTableContent + "</tr>";
            }
            return Json(historyLogTableContent, JsonRequestBehavior.AllowGet);
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

        public ActionResult RevertChanges(long HistLogID, int type, string actionType)
        {
            if (actionType == "update")
            {
                CCHistoryLog HistLogObj = CCHistoryLogRepository.CCHistoryLogs.Where(hist => hist.ID == HistLogID).FirstOrDefault();
                CCFieldValue FieldValObj = CCFieldValueRepository.CCFieldValues.Where(field => field.FieldID == HistLogObj.FieldID && field.ItemID == HistLogObj.ItemID).FirstOrDefault();

                FieldValObj.Value = HistLogObj.OldValue;
                FieldValObj.LastUpdated = DateTime.Now;
                CCFieldValueRepository.RevertChangeToFieldValues(FieldValObj, HistLogObj);

                CCItems ItemObj = CCItemRepository.CCContacts.Where(item => item.ItemID == HistLogObj.ItemID).FirstOrDefault();

                if (type == 1)
                    return RedirectToAction("Items", "Folder", new { id = ItemObj.FolderID });
                else
                    return RedirectToAction("Items", "Folder", new { id = ItemObj.FolderID }); //have to cahnge for Item view.. Need 3 parameterd.. pid?
            }
            else if (actionType == "delete")
            {
                //Need to complete for DELETE
                CCHistoryLog HistLogObj = CCHistoryLogRepository.CCHistoryLogs.Where(hist => hist.ID == HistLogID).FirstOrDefault();
                CCItems ItemObj = CCItemRepository.CCContacts.Where(item => item.ItemID == HistLogObj.ItemID).FirstOrDefault();
                CCItems ItemTempObj = new CCItems();
                ItemTempObj.ItemID = ItemObj.ItemID;
                ItemTempObj.DeDupeValue = ItemObj.DeDupeValue;
                if (ItemObj.isDeleted == true)
                    ItemTempObj.isDeleted = false;
                CCItemRepository.UpdateContact(ItemTempObj);

                return RedirectToAction("Items", "Folder", new { id = ItemObj.FolderID });
            }
            else
            {
                CCHistoryLog HistLogObj = CCHistoryLogRepository.CCHistoryLogs.Where(hist => hist.ID == HistLogID).FirstOrDefault();
                CCItems ItemObj = CCItemRepository.CCContacts.Where(item => item.ItemID == HistLogObj.ItemID).FirstOrDefault();
                return RedirectToAction("Items", "Folder", new { id = ItemObj.FolderID });
            }            
        }

        public ActionResult currentConnectionBeingAdded()
        {
            return Json(conectionBeingAddedEmail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult setUpNewConnection()
        {
            ExLogOnViewModel modelObj = (ExLogOnViewModel)Session["NewCredentialObject"];
            return View(modelObj);
        }

        public ActionResult viewAppointments(long ID)
        {
            Account accountObj = (Account)Session["account"];
            AppointmentViewModel AppointmentViewModel = new AppointmentViewModel();
            bool IsCrimeDiaryFields = false;

            TempData["SelectedMenu"] = "Folder";
            TempData["SelectedSubMenuFolder"] = ID;

            CCFolder folderDetail = new CCFolder();
            folderDetail = CCFolderRepository.FolderDetails(ID);
            AppointmentViewModel.FolderDetails = folderDetail;

            Session["folderDetail"] = folderDetail;
            LimitationsViewModel limitationsObj = (LimitationsViewModel)Session["limitations"];
            AppointmentViewModel.limitationsObj = limitationsObj;

            //get folderfileds             
            List<CCFolderField> folderfields = new List<CCFolderField>();
            folderfields = CCFieldRepository.CCFolderFields.Where(id => id.FolderID == ID & id.AccountGUID == accountObj.AccountGUID).ToList();
            var isExisting = folderfields.Where(cap => cap.FieldName == "LawyerName" & cap.AccountGUID == accountObj.AccountGUID).FirstOrDefault();
            if (isExisting != null) { IsCrimeDiaryFields = true; }
            else { IsCrimeDiaryFields = false; }
            AppointmentViewModel.IsCrimeDiaryFields = IsCrimeDiaryFields;

            AppointmentViewModel.AppointmentListObjs = new List<customAppointmentView>();

            var ItemListForFolder = CCItemRepository.CCContacts.Where(i => i.FolderID == ID & i.isDeleted == false).ToList();

            foreach (var item in ItemListForFolder)
            {
                customAppointmentView C = new customAppointmentView();
                C.ItemID = item.ItemID;
                var FolderFieldID = folderfields.Where(ff=>ff.FieldCaption == "Subject").First().FieldID;
                C.subject = CCFieldValueRepository.CCFieldValues.Where(fv => fv.ItemID == item.ItemID && fv.FieldID == FolderFieldID).First().Value;
                FolderFieldID = folderfields.Where(ff => ff.FieldCaption == "Start Time").First().FieldID;
                var itemStartDate = CCFieldValueRepository.CCFieldValues.Where(fv => fv.ItemID == item.ItemID && fv.FieldID == FolderFieldID).First().Value;

                //DateTime itemStartDateMain = DateTime.Parse(itemStartDate);

                DateTime timeUtc = DateTime.Parse(itemStartDate);
                DateTime cstTime = new DateTime();
                try
                {
                    TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(accountObj.TimeZone.ToString());
                    cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
                    Console.WriteLine("The date and time are {0} {1}.",
                                      cstTime,
                                      cstZone.IsDaylightSavingTime(cstTime) ?
                                              cstZone.DaylightName : cstZone.StandardName);
                }
                catch (TimeZoneNotFoundException)
                {
                    Console.WriteLine("Timezone conversion error");
                }
                catch (InvalidTimeZoneException)
                {
                    Console.WriteLine("conn.LastSyncTime");
                }

                DateTime itemStartDateMain = DateTime.Parse(cstTime.ToString());
                
                C.startYear = itemStartDateMain.Year;
                C.startMonth = itemStartDateMain.Month;
                C.startDay = itemStartDateMain.Day;
                C.startHrs = itemStartDateMain.Hour;
                C.startMins = itemStartDateMain.Minute;

                FolderFieldID = folderfields.Where(ff => ff.FieldCaption == "End Time").First().FieldID;
                var itemEndDate = CCFieldValueRepository.CCFieldValues.Where(fv => fv.ItemID == item.ItemID && fv.FieldID == FolderFieldID).First().Value;

                //DateTime itemEndDateMain = DateTime.Parse(itemEndDate);

                timeUtc = DateTime.Parse(itemEndDate);
                cstTime = new DateTime();
                try
                {
                    TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(accountObj.TimeZone.ToString());
                    cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
                    Console.WriteLine("The date and time are {0} {1}.",
                                      cstTime,
                                      cstZone.IsDaylightSavingTime(cstTime) ?
                                              cstZone.DaylightName : cstZone.StandardName);
                }
                catch (TimeZoneNotFoundException)
                {
                    Console.WriteLine("Timezone conversion error");
                }
                catch (InvalidTimeZoneException)
                {
                    Console.WriteLine("conn.LastSyncTime");
                }

                DateTime itemEndDateMain = DateTime.Parse(cstTime.ToString());

                C.endYear = itemEndDateMain.Year;
                C.endMonth = itemEndDateMain.Month;
                C.endDay = itemEndDateMain.Day;
                C.endHrs = itemEndDateMain.Hour;
                C.endMins = itemEndDateMain.Minute;

                AppointmentViewModel.AppointmentListObjs.Add(C);
            }


            return View(AppointmentViewModel);   
        }

        public ActionResult AppointmentListView(long ID)
        {
            Account accountObj = (Account)Session["account"];
            AppointmentViewModel AppointmentViewModel = new AppointmentViewModel();
            bool IsCrimeDiaryFields = false;

            TempData["SelectedMenu"] = "Folder";
            TempData["SelectedSubMenuFolder"] = ID;

            CCFolder folderDetail = new CCFolder();
            folderDetail = CCFolderRepository.FolderDetails(ID);
            AppointmentViewModel.FolderDetails = folderDetail;

            Session["folderDetail"] = folderDetail;
            LimitationsViewModel limitationsObj = (LimitationsViewModel)Session["limitations"];
            AppointmentViewModel.limitationsObj = limitationsObj;

            //get folderfileds             
            List<CCFolderField> folderfields = new List<CCFolderField>();
            folderfields = CCFieldRepository.CCFolderFields.Where(id => id.FolderID == ID & id.AccountGUID == accountObj.AccountGUID).ToList();
            var isExisting = folderfields.Where(cap => cap.FieldName == "LawyerName" & cap.AccountGUID == accountObj.AccountGUID).FirstOrDefault();
            if (isExisting != null) { IsCrimeDiaryFields = true; }
            else { IsCrimeDiaryFields = false; }
            AppointmentViewModel.IsCrimeDiaryFields = IsCrimeDiaryFields;

            AppointmentViewModel.AppointmentListObjs = new List<customAppointmentView>();

            var ItemListForFolder = CCItemRepository.CCContacts.Where(i => i.FolderID == ID & i.isDeleted == false).ToList();

            foreach (var item in ItemListForFolder)
            {
                customAppointmentView C = new customAppointmentView();
                C.ItemID = item.ItemID;
                var FolderFieldID = folderfields.Where(ff => ff.FieldCaption == "Subject").First().FieldID;
                C.subject = CCFieldValueRepository.CCFieldValues.Where(fv => fv.ItemID == item.ItemID && fv.FieldID == FolderFieldID).First().Value;
                FolderFieldID = folderfields.Where(ff => ff.FieldCaption == "Start Time").First().FieldID;
                var itemStartDate = CCFieldValueRepository.CCFieldValues.Where(fv => fv.ItemID == item.ItemID && fv.FieldID == FolderFieldID).First().Value;

                //DateTime itemStartDateMain = DateTime.Parse(itemStartDate);

                DateTime timeUtc = DateTime.Parse(itemStartDate);
                DateTime cstTime = new DateTime();
                try
                {
                    TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(accountObj.TimeZone.ToString());
                    cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
                    Console.WriteLine("The date and time are {0} {1}.",
                                      cstTime,
                                      cstZone.IsDaylightSavingTime(cstTime) ?
                                              cstZone.DaylightName : cstZone.StandardName);
                }
                catch (TimeZoneNotFoundException)
                {
                    Console.WriteLine("Timezone conversion error");
                }
                catch (InvalidTimeZoneException)
                {
                    Console.WriteLine("conn.LastSyncTime");
                }

                DateTime itemStartDateMain = DateTime.Parse(cstTime.ToString());

                C.startYear = itemStartDateMain.Year;
                C.startMonth = itemStartDateMain.Month;
                C.startDay = itemStartDateMain.Day;
                C.startHrs = itemStartDateMain.Hour;
                C.startMins = itemStartDateMain.Minute;

                FolderFieldID = folderfields.Where(ff => ff.FieldCaption == "End Time").First().FieldID;
                var itemEndDate = CCFieldValueRepository.CCFieldValues.Where(fv => fv.ItemID == item.ItemID && fv.FieldID == FolderFieldID).First().Value;

                //DateTime itemEndDateMain = DateTime.Parse(itemEndDate);

                timeUtc = DateTime.Parse(itemEndDate);
                cstTime = new DateTime();
                try
                {
                    TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(accountObj.TimeZone.ToString());
                    cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
                    Console.WriteLine("The date and time are {0} {1}.",
                                      cstTime,
                                      cstZone.IsDaylightSavingTime(cstTime) ?
                                              cstZone.DaylightName : cstZone.StandardName);
                }
                catch (TimeZoneNotFoundException)
                {
                    Console.WriteLine("Timezone conversion error");
                }
                catch (InvalidTimeZoneException)
                {
                    Console.WriteLine("conn.LastSyncTime");
                }

                DateTime itemEndDateMain = DateTime.Parse(cstTime.ToString());

                C.endYear = itemEndDateMain.Year;
                C.endMonth = itemEndDateMain.Month;
                C.endDay = itemEndDateMain.Day;
                C.endHrs = itemEndDateMain.Hour;
                C.endMins = itemEndDateMain.Minute;

                AppointmentViewModel.AppointmentListObjs.Add(C);
            }


            return View(AppointmentViewModel);   
        }

    }
}
