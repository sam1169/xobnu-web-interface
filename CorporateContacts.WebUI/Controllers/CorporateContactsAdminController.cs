using Xobnu.Domain.Abstract;
using Xobnu.WebUI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xobnu.Domain.Entities;
using Xobnu.WebUI.Models;
using Xobnu.WebUI.Util;

namespace Xobnu.WebUI.Controllers
{
    [Authorize]
    [CheckSessionOutAttribute]
    [CheckUserIsAdmin]
    [HandleError]
    public class CorporateContactsAdminController : Controller
    {
        ICCConnectionsRepo CCConnectionRepository;
        IAccountRepo CCAccRepository;
        IUserRepo CCUserRepository;
        ICCErrorLogRepo CCErrorLogRepository;
        ICCFolderRepo CCFolderRepository;
        ICCItemRepo CCItemRepository;

        ICCFolderFieldRepo CCFieldRepository;
        ICCFieldValuesRepo CCFieldValueRepository;
        ICCGroupRepo CCGroupRepository;
        ICCGroupFieldRepo CCGroupFieldRepository;
        ICCLayoutRepo CCLayoutRepository;
        ICCLayoutGroupRepo CCLayoutGroupRepository;
        ICredentialRepo CCCredentialRepository;
        ICCFieldMappingsRepo CCFieldMappingsRepository;
        ICCNoteRepo CCNoteRepository;

        ICCSyncFieldsRepo CCSyncFieldsRepository;
        ICCSyncItems CCSyncItemsRepository;

        ICCHealthMsgs CCHealthMsgsRepository;
        //
        // GET: /CorporateContactsAdmin/
        public CorporateContactsAdminController(ICCConnectionsRepo connection, IAccountRepo account, ICCErrorLogRepo errLog, ICCFolderRepo folder, ICCItemRepo items,
            ICCFolderFieldRepo field,ICCFieldValuesRepo fieldvalue, ICCGroupRepo group, ICCGroupFieldRepo groupfield, ICCLayoutRepo layout, ICCLayoutGroupRepo layoutgroup,
            ICredentialRepo credential, ICCFieldMappingsRepo fieldmapping, ICCNoteRepo note, ICCSyncFieldsRepo syncFields, ICCSyncItems syncItems, IUserRepo user, ICCHealthMsgs healthMsgs)
        {
            CCConnectionRepository = connection;
            CCAccRepository = account;
            CCErrorLogRepository = errLog;
            CCFolderRepository = folder;
            CCItemRepository = items;
            CCFieldRepository = field;
            CCFieldValueRepository = fieldvalue;
            CCGroupRepository = group;
            CCGroupFieldRepository = groupfield;
            CCLayoutRepository = layout;
            CCLayoutGroupRepository = layoutgroup;
            CCCredentialRepository = credential;
            CCFieldMappingsRepository = fieldmapping;
            CCNoteRepository = note;
            CCSyncFieldsRepository = syncFields;
            CCSyncItemsRepository = syncItems;
            CCUserRepository = user;
            CCHealthMsgsRepository = healthMsgs;
        }

        public ActionResult Index()
        {
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];

            var model = new SystemAdminDashboardViewModel();

            model.FolderCount = CCFolderRepository.CCFolders.Count();
            model.ConnectionCount = CCConnectionRepository.CCSubscriptions.Count();
            model.UserCount = CCUserRepository.Users.Count();
            model.AccountCount = CCAccRepository.Accounts.Count();
            model.ItemCount = CCItemRepository.CCContacts.Count();

            DateTime DateStart = DateTime.Now.Date.AddDays(-3);
            DateTime DateEnd = DateTime.Now.Date;

            model.ErrorLogHistoryLimited = new List<CCErrorLog>();
            var query = CCErrorLogRepository.CCErrorLogs.OrderByDescending(x => x.DateTime).Take(100).ToList();
            model.ErrorLogHistoryLimited = CCErrorLogRepository.CCErrorLogs.OrderByDescending(x=>x.DateTime).Take(100).ToList();


            

            var assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ViewBag.version = assemblyVersion;
            Session["version"] = assemblyVersion;

            //model.SelectedAccountID = 0;
            return View(model);
        }


        //public ActionResult Index(SystemAdminDashboardViewModel obj)
        //{

        //    return View(obj);
        //}

        public ActionResult AccountInfo()
        {
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];

            TempData["SelectedMenu"] = "accInfo";

            var model = new SystemAdminDashboardViewModel();

            Session["ErrorSourceFilterList"] = model.ErrorSourceFilterList;
            Session["ErrorTypeFilterList"] = model.ErrorTypeFilterList;

            if (Session["SystemAdminDashboardViewModel"] != null)
            {
                model = (SystemAdminDashboardViewModel)Session["SystemAdminDashboardViewModel"];
                model.ExistingAccounts = CCAccRepository.Accounts.ToList();

            }
            else
            {
                model.ExistingAccounts = CCAccRepository.Accounts.ToList();
            }

            var assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ViewBag.version = assemblyVersion;
            Session["version"] = assemblyVersion;

            //model.SelectedAccountID = 0;
            return View(model);
        
        }

        public ActionResult ClearFlageAndPauseTime()
        {
            // get all account details 
            var connectionStrings = CCAccRepository.Accounts.ToList();
            foreach (var dbConnection in connectionStrings)
            {

            }
            return View();
        }

        
        public ActionResult ViewConnectionDetails(long ConnectionID, long AccID)
        {
            var model = new SystemAdminDashboardViewModel();

            model.SelectedAccountID = AccID;
            var selectedAccount = CCAccRepository.Accounts.Where(acc => acc.ID == model.SelectedAccountID);

            foreach (var acc in selectedAccount)
            {
                model.SelectedConnectionID = ConnectionID;
                model.SelectedConnection = new SelectedConnectionDetails();
                model.SelectedConnection.Connection = CCConnectionRepository.CCSubscriptions.Where(conn => conn.AccountGUID == acc.AccountGUID && conn.ConnectionID == model.SelectedConnectionID).FirstOrDefault();
                model.SelectedConnection.Folder = CCFolderRepository.CCFolders.Where(fold => fold.FolderID == model.SelectedConnection.Connection.FolderID && fold.AccountGUID == acc.AccountGUID).FirstOrDefault();

                model.SelectedConnectionCheckBoxes = new ConnectionCheckBoxToggles();

                if (model.SelectedConnection.Connection.AllowAdditions == true)
                    model.SelectedConnectionCheckBoxes.allowAdditions = "checked";
                if (model.SelectedConnection.Connection.IgnoreExisting == true)
                    model.SelectedConnectionCheckBoxes.ignoreExsisting = "checked";
                if (model.SelectedConnection.Connection.CategoryFilterUsed == true)
                    model.SelectedConnectionCheckBoxes.categoryFilter = "checked";
                if (model.SelectedConnection.Connection.CopyPhotos == true)
                    model.SelectedConnectionCheckBoxes.copyPhotos = "checked";
                if (model.SelectedConnection.Connection.TurnOffReminders == true)
                    model.SelectedConnectionCheckBoxes.turnOffReminders = "checked";
                if (model.SelectedConnection.Connection.tagSubject == true)
                    model.SelectedConnectionCheckBoxes.tagSubject = "checked";
                if (model.SelectedConnection.Connection.IsRunning == true)
                    model.SelectedConnectionCheckBoxes.isRunning = "checked";
                if (model.SelectedConnection.Connection.IsPaused == true)
                    model.SelectedConnectionCheckBoxes.isPaused = "checked";


            }
            return View(model);
        }

        
        public ActionResult ChangeConnectionToggleButtons(long ConnectionID, string chkbox)
        {

            CCConnection connObj = new CCConnection();
            connObj.ConnectionID = ConnectionID;
            connObj = CCConnectionRepository.CCSubscriptions.Where(conn => conn.ConnectionID == connObj.ConnectionID).FirstOrDefault();

            if (chkbox == "RestartTimer")
            {
                connObj.LastSyncTime = "1753-01-01 00:00";
            }
            else
            {
                if (chkbox == "AllowAdditions")
                {
                    if (connObj.AllowAdditions == true)
                        connObj.AllowAdditions = false;
                    else
                        connObj.AllowAdditions = true;
                }

                if (chkbox == "IgnoreExisting")
                {
                    if (connObj.IgnoreExisting == true)
                        connObj.IgnoreExisting = false;
                    else
                        connObj.IgnoreExisting = true;
                }

                if (chkbox == "CategoryFilterUsed")
                {
                    if (connObj.CategoryFilterUsed == true)
                        connObj.CategoryFilterUsed = false;
                    else
                        connObj.CategoryFilterUsed = true;
                }

                if (chkbox == "CopyPhotos")
                {
                    if (connObj.CopyPhotos == true)
                        connObj.CopyPhotos = false;
                    else
                        connObj.CopyPhotos = true;
                }

                if (chkbox == "TurnOffReminders")
                {
                    if (connObj.TurnOffReminders == true)
                        connObj.TurnOffReminders = false;
                    else
                        connObj.TurnOffReminders = true;
                }

                if (chkbox == "tagSubject")
                {
                    if (connObj.tagSubject == true)
                        connObj.tagSubject = false;
                    else
                        connObj.tagSubject = true;
                }
                if (chkbox == "IsRunning")
                {
                    if (connObj.IsRunning == true)
                        connObj.IsRunning = false;
                    else
                        connObj.IsRunning = true;
                }
                if (chkbox == "IsPaused")
                {
                    if (connObj.IsPaused == true)
                        connObj.IsPaused = false;
                    else
                        connObj.IsPaused = true;
                }
            }
            
            bool update = CCConnectionRepository.UpdateConnectionBooleanToggles(connObj);

            Account AccObj = CCAccRepository.Accounts.Where(acc => acc.AccountGUID == connObj.AccountGUID).FirstOrDefault();

            return RedirectToAction("ViewConnectionDetails", "CorporateContactsAdmin", new { ConnectionID = connObj.ConnectionID, AccID = AccObj.ID});
        }

        
        public ActionResult FilterErrorList(long AccountID, string ErrorFilter, long FilterType, string redirectMethod)
        {
            var model = new SystemAdminDashboardViewModel();

            if (Session["SystemAdminDashboardViewModel"] != null)
            {
                model = (SystemAdminDashboardViewModel)Session["SystemAdminDashboardViewModel"];
            }


            if (FilterType == 1)
            {
                //List<ErrorFilters> ErrorSourceFilterList = (List<ErrorFilters>)Session["ErrorSourceFilterList"];
                foreach (var err in model.ErrorSourceFilterList)
                {
                    if (ErrorFilter == err.Error)
                    {
                        if(err.ErrorState == "active")
                            err.ErrorState = "disabled";
                        else
                            err.ErrorState = "active";
                    }
                }
            }
            else if (FilterType == 2)
            {
                //List<ErrorFilters> ErrorTypeFilterList = (List<ErrorFilters>)Session["ErrorTypeFilterList"];
                foreach (var err in model.ErrorTypeFilterList)
                {
                    if (ErrorFilter == err.Error)
                    {
                        if (err.ErrorState == "active")
                            err.ErrorState = "disabled";
                        else
                            err.ErrorState = "active";
                    }
                }
            }

            
            model.SelectedAccountID = AccountID;
            model.SelectedInformationType = 2;

             var selectedAccount = CCAccRepository.Accounts.Where(acc => acc.ID == model.SelectedAccountID);

             foreach (var acc in selectedAccount)
             {
                 if (model.SelectedInformationType == 2) //Error List
                 {
                     model.ErrorLogList = CCErrorLogRepository.CCErrorLogs.Where(error => error.AccountGUID == acc.AccountGUID).Take(100).ToList();
                     model.ErrorLogList = FilteredErrorLogList(model.ErrorLogList, model.ErrorSourceFilterList, model.ErrorTypeFilterList);
                     //model.ErrorSourceFilterList = (List<ErrorFilters>)Session["ErrorSourceFilterList"];
                     //model.ErrorTypeFilterList = (List<ErrorFilters>)Session["ErrorTypeFilterList"];
                 }
             }

            Session["SystemAdminDashboardViewModel"] = model;

            if (redirectMethod == "index")  
                return RedirectToAction("Index", "CorporateContactsAdmin");
            else
                return RedirectToAction("viewMasterErrorLog", "CorporateContactsAdmin");

            
        }


        private List<CCErrorLog> FilteredErrorLogList(List<CCErrorLog> ErrorLogList, List<ErrorFilters> ErrorSourceFilterList, List<ErrorFilters> ErrorTypeFilterList)
        {
            

            foreach (var err in ErrorSourceFilterList)
            {
                if(err.ErrorState == "disabled")
                {
                    ErrorLogList.RemoveAll(rem => rem.Source == err.Error);
                }
            }

            foreach (var err in ErrorTypeFilterList)
            {
                if (err.ErrorState == "disabled")
                {
                    ErrorLogList.RemoveAll(rem => rem.ErrorType + " Error" == err.Error);
                }
            }

            return ErrorLogList;
        }

        public ActionResult getStackTrace(long ErrLogID)
        {

            CCErrorLog ErrLog = new CCErrorLog();
            ErrLog = CCErrorLogRepository.CCErrorLogs.Where(error => error.LogID == ErrLogID).FirstOrDefault();

            if(ErrLog.ErrorStackTrace == null)
                return Json("Not Found", JsonRequestBehavior.AllowGet);
            else
                return Json(ErrLog.ErrorStackTrace.ToString(), JsonRequestBehavior.AllowGet);

            
        }

        public ActionResult viewMasterErrorLog()
        {
            var model = new SystemAdminDashboardViewModel();

            if (Session["SystemAdminDashboardViewModel"] != null)
            {
                model = (SystemAdminDashboardViewModel)Session["SystemAdminDashboardViewModel"];
                model.ErrorLogList = CCErrorLogRepository.CCErrorLogs.OrderByDescending(e => e.DateTime).Take(200).ToList();  
            }
            else
            {
                model.ErrorLogList = CCErrorLogRepository.CCErrorLogs.OrderByDescending(e => e.DateTime).Take(200).ToList();                
            } 
            model.ErrorLogList = FilteredErrorLogList(model.ErrorLogList, model.ErrorSourceFilterList, model.ErrorTypeFilterList);

            Session["SystemAdminDashboardViewModel"] = model;

            return View(model);
        }

        [HttpPost]
        public ActionResult GenerateAccountDetails(SystemAdminDashboardViewModel obj)
        {
          

            var model = new SystemAdminDashboardViewModel();
            model.SelectedAccountID = obj.SelectedAccountID;
            model.SelectedInformationType = obj.SelectedInformationType;
            var selectedAccount = CCAccRepository.Accounts.Where(acc => acc.ID == model.SelectedAccountID);

            foreach (var acc in selectedAccount)
            {

                if (model.SelectedInformationType == 2) //Error List
                {
                    model.ErrorLogList = CCErrorLogRepository.CCErrorLogs.Where(error => error.AccountGUID == acc.AccountGUID).Take(100).ToList();
                    model.ErrorLogList = FilteredErrorLogList(model.ErrorLogList, model.ErrorSourceFilterList, model.ErrorTypeFilterList);
                    //model.ErrorSourceFilterList = (List<ErrorFilters>)Session["ErrorSourceFilterList"];
                    //model.ErrorTypeFilterList = (List<ErrorFilters>)Session["ErrorTypeFilterList"];
                }
                else if (model.SelectedInformationType == 1) //Folder List
                {
                    model.Folders = CCFolderRepository.CCFolders.Where(guid => guid.AccountGUID == acc.AccountGUID).ToList();
                    model.FolderList = new List<FolderDetails>();
                    foreach (var folder in model.Folders)
                    {
                        FolderDetails FD = new FolderDetails();
                        FD.FolderName = folder.Name;
                        FD.ItemCount = CCItemRepository.CCContacts.Where(items => items.FolderID == folder.FolderID).Count();
                        model.FolderList.Add(FD);
                    }
                }
                else if (model.SelectedInformationType == 3) //Connection List
                {
                    model.Connections = CCConnectionRepository.CCSubscriptions.Where(conn => conn.AccountGUID == acc.AccountGUID).ToList();
                    model.ConnectionList = new List<ConnectionDetails>();
                    foreach (var conn in model.Connections)
                    {
                        ConnectionDetails CD = new ConnectionDetails();
                        CD.Connection = conn;
                        CD.Folders = CCFolderRepository.CCFolders.Where(fold => fold.FolderID == conn.FolderID && fold.AccountGUID == acc.AccountGUID).FirstOrDefault();
                        model.ConnectionList.Add(CD);
                    }
                }
                else if (model.SelectedInformationType == 4) //Database Usage
                {
                    double TotalTableUsage = 0;
                    double TotalTableUsageForAccount = 0;

                    model.TableUsageList = new List<DBTableUsageDetails>();
                    

                    //Connection Table
                    TotalTableUsage = CCConnectionRepository.CCSubscriptions.Count();
                    TotalTableUsageForAccount = CCConnectionRepository.CCSubscriptions.Where(conn => conn.AccountGUID == acc.AccountGUID).Count();
                    DBTableUsageDetails tblConnectionObj = new DBTableUsageDetails();
                    tblConnectionObj.TableName = "Connections";
                    tblConnectionObj.TableUsage = Math.Round((TotalTableUsageForAccount / TotalTableUsage) * 100, 2);
                    tblConnectionObj.RecordCount = TotalTableUsageForAccount + " of " + TotalTableUsage;
                    model.TableUsageList.Add(tblConnectionObj);

                    //Credentials Table
                    TotalTableUsage = CCCredentialRepository.Credentials.Count();
                    TotalTableUsageForAccount = CCCredentialRepository.Credentials.Where(cred => cred.AccountGUID == acc.AccountGUID).Count();
                    DBTableUsageDetails tblCredentialsObj = new DBTableUsageDetails();
                    tblCredentialsObj.TableName = "Credentials";
                    tblCredentialsObj.TableUsage = Math.Round((TotalTableUsageForAccount / TotalTableUsage) * 100, 2);
                    tblCredentialsObj.RecordCount = TotalTableUsageForAccount + " of " + TotalTableUsage;
                    model.TableUsageList.Add(tblCredentialsObj);

                    //Field Mappings Table
                    TotalTableUsage = CCFieldMappingsRepository.CCFieldsMapping.Count();
                    TotalTableUsageForAccount = CCFieldMappingsRepository.CCFieldsMapping.Where(fieldMappings => fieldMappings.AccountGUID == acc.AccountGUID).Count();
                    DBTableUsageDetails tblFieldMappingsObj = new DBTableUsageDetails();
                    tblFieldMappingsObj.TableName = "FieldMappings";
                    tblFieldMappingsObj.TableUsage = Math.Round((TotalTableUsageForAccount / TotalTableUsage) * 100, 2);
                    tblFieldMappingsObj.RecordCount = TotalTableUsageForAccount + " of " + TotalTableUsage;
                    model.TableUsageList.Add(tblFieldMappingsObj);

                    //Field Values Table
                    TotalTableUsage = CCFieldValueRepository.CCFieldValues.Count();
                    TotalTableUsageForAccount = CCFieldValueRepository.CCFieldValues.Where(fieldValues => fieldValues.AccountGUID == acc.AccountGUID).Count();
                    DBTableUsageDetails tblFieldValuesObj = new DBTableUsageDetails();
                    tblFieldValuesObj.TableName = "FieldValues";
                    tblFieldValuesObj.TableUsage = Math.Round((TotalTableUsageForAccount / TotalTableUsage) * 100, 2);
                    tblFieldValuesObj.RecordCount = TotalTableUsageForAccount + " of " + TotalTableUsage;
                    model.TableUsageList.Add(tblFieldValuesObj);

                    //Folder Fields Table
                    TotalTableUsage = CCFieldValueRepository.CCFieldValues.Count();
                    TotalTableUsageForAccount = CCFieldValueRepository.CCFieldValues.Where(folderFields => folderFields.AccountGUID == acc.AccountGUID).Count();
                    DBTableUsageDetails tblFolderFieldsObj = new DBTableUsageDetails();
                    tblFolderFieldsObj.TableName = "Folder Fields";
                    tblFolderFieldsObj.TableUsage = Math.Round((TotalTableUsageForAccount / TotalTableUsage) * 100, 2);
                    tblFolderFieldsObj.RecordCount = TotalTableUsageForAccount + " of " + TotalTableUsage;
                    model.TableUsageList.Add(tblFolderFieldsObj);

                    //Folders Table
                    TotalTableUsage = CCFolderRepository.CCFolders.Count();
                    TotalTableUsageForAccount = CCFolderRepository.CCFolders.Where(folders => folders.AccountGUID == acc.AccountGUID).Count();
                    DBTableUsageDetails tblFoldersObj = new DBTableUsageDetails();
                    tblFoldersObj.TableName = "Folders";
                    tblFoldersObj.TableUsage = Math.Round((TotalTableUsageForAccount / TotalTableUsage) * 100, 2);
                    tblFoldersObj.RecordCount = TotalTableUsageForAccount + " of " + TotalTableUsage;
                    model.TableUsageList.Add(tblFoldersObj);

                    //Group Fields Table
                    TotalTableUsage = CCGroupFieldRepository.CCGroupsFields.Count();
                    TotalTableUsageForAccount = CCGroupFieldRepository.CCGroupsFields.Where(groupFields => groupFields.AccountGUID == acc.AccountGUID).Count();
                    DBTableUsageDetails tblGroupFieldsObj = new DBTableUsageDetails();
                    tblGroupFieldsObj.TableName = "Group Fields";
                    tblGroupFieldsObj.TableUsage = Math.Round((TotalTableUsageForAccount / TotalTableUsage) * 100, 2);
                    tblGroupFieldsObj.RecordCount = TotalTableUsageForAccount + " of " + TotalTableUsage;
                    model.TableUsageList.Add(tblGroupFieldsObj);

                    //Groups Table
                    TotalTableUsage = CCGroupRepository.CCGroups.Count();
                    TotalTableUsageForAccount = CCGroupRepository.CCGroups.Where(groups => groups.AccountGUID == acc.AccountGUID).Count();
                    DBTableUsageDetails tblGroupsObj = new DBTableUsageDetails();
                    tblGroupsObj.TableName = "Groups";
                    tblGroupsObj.TableUsage = Math.Round((TotalTableUsageForAccount / TotalTableUsage) * 100, 2);
                    tblGroupsObj.RecordCount = TotalTableUsageForAccount + " of " + TotalTableUsage;
                    model.TableUsageList.Add(tblGroupsObj);

                    //Items Table
                    TotalTableUsage = CCItemRepository.CCContacts.Count();
                    TotalTableUsageForAccount = CCItemRepository.CCContacts.Where(items => items.AccountGUID == acc.AccountGUID).Count();
                    DBTableUsageDetails tblItemsObj = new DBTableUsageDetails();
                    tblItemsObj.TableName = "Items";
                    tblItemsObj.TableUsage = Math.Round((TotalTableUsageForAccount / TotalTableUsage) * 100, 2);
                    tblItemsObj.RecordCount = TotalTableUsageForAccount + " of " + TotalTableUsage;
                    model.TableUsageList.Add(tblItemsObj);

                    //Layout Groups Table
                    TotalTableUsage = CCLayoutGroupRepository.CCLayoutGroups.Count();
                    TotalTableUsageForAccount = CCLayoutGroupRepository.CCLayoutGroups.Where(layoutGroup => layoutGroup.AccountGUID == acc.AccountGUID).Count();
                    DBTableUsageDetails tblLayoutGroupsObj = new DBTableUsageDetails();
                    tblLayoutGroupsObj.TableName = "Layout Groups";
                    tblLayoutGroupsObj.TableUsage = Math.Round((TotalTableUsageForAccount / TotalTableUsage) * 100, 2);
                    tblLayoutGroupsObj.RecordCount = TotalTableUsageForAccount + " of " + TotalTableUsage;
                    model.TableUsageList.Add(tblLayoutGroupsObj);

                    //Layouts Table
                    TotalTableUsage = CCLayoutRepository.CCLayouts.Count();
                    TotalTableUsageForAccount = CCLayoutRepository.CCLayouts.Where(layouts => layouts.AccountGUID == acc.AccountGUID).Count();
                    DBTableUsageDetails tblLayoutsObj = new DBTableUsageDetails();
                    tblLayoutsObj.TableName = "Layouts";
                    tblLayoutsObj.TableUsage = Math.Round((TotalTableUsageForAccount / TotalTableUsage) * 100, 2);
                    tblLayoutsObj.RecordCount = TotalTableUsageForAccount + " of " + TotalTableUsage;
                    model.TableUsageList.Add(tblLayoutsObj);

                    //Notes Table
                    TotalTableUsage = CCNoteRepository.CCNotes.Count();
                    TotalTableUsageForAccount = CCNoteRepository.CCNotes.Where(notes => notes.AccountGUID == acc.AccountGUID).Count();
                    DBTableUsageDetails tblNotesObj = new DBTableUsageDetails();
                    tblNotesObj.TableName = "Notes";
                    tblNotesObj.TableUsage = Math.Round((TotalTableUsageForAccount / TotalTableUsage) * 100, 2);
                    tblNotesObj.RecordCount = TotalTableUsageForAccount + " of " + TotalTableUsage;
                    model.TableUsageList.Add(tblNotesObj);

                    ////Sync Fields Table
                    //TotalTableUsage = CCSyncFieldsRepository.CCSyncFields.Count();
                    //TotalTableUsageForAccount = CCSyncFieldsRepository.CCSyncFields.Where(cred => cred.AccountGUID == acc.AccountGUID).Count();
                    //DBTableUsageDetails tblSyncFieldsObj = new DBTableUsageDetails();
                    //tblSyncFieldsObj.TableName = "SyncFields";
                    //tblSyncFieldsObj.TableUsage = Math.Round((TotalTableUsageForAccount / TotalTableUsage) * 100, 2);
                    //tblSyncFieldsObj.RecordCount = TotalTableUsageForAccount + " of " + TotalTableUsage;
                    //model.TableUsageList.Add(tblSyncFieldsObj);

                    ////Sync Items Table
                    //TotalTableUsage = CCSyncItemsRepository.CCSyncItems.Count();
                    //TotalTableUsageForAccount = CCSyncItemsRepository.CCSyncItems.Where(cred => cred.AccountGUID == acc.AccountGUID).Count();
                    //DBTableUsageDetails tblSyncItemsObj = new DBTableUsageDetails();
                    //tblSyncItemsObj.TableName = "SyncItems";
                    //tblSyncItemsObj.TableUsage = Math.Round((TotalTableUsageForAccount / TotalTableUsage) * 100, 2);
                    //tblSyncItemsObj.RecordCount = TotalTableUsageForAccount + " of " + TotalTableUsage;
                    //model.TableUsageList.Add(tblSyncItemsObj);


                }
                Session["SystemAdminDashboardViewModel"] = model;
            }


            return RedirectToAction("AccountInfo", "CorporateContactsAdmin");
        }

        public ActionResult ImpersonateAccount()
        {       
            if (Session["SysAdminDetails"] != null)
            {
                TempData["SelectedMenu"] = "AccImpersontation";
                var adminUser = (User)Session["SysAdminDetails"];

                var userObj = CCUserRepository.GetUserByEmailAddress(adminUser.Email);
                Session["user"] = userObj;
                //Session["account"] = "";
                //Session["timeZone"] = "";
                var accountObj = CCAccRepository.Accounts.FirstOrDefault(x => x.ID == userObj.AccountID);
                Session["account"] = accountObj;
                var timeZone = CCAccRepository.Accounts.Where(aid => aid.ID == userObj.AccountID).FirstOrDefault().TimeZone;
                Session["timeZone"] = timeZone;


                ImpersonateAccountsSysAdminModel model = new ImpersonateAccountsSysAdminModel();

                var AccountsList = CCAccRepository.Accounts.ToList();

                foreach (var acc in AccountsList)
                {

                    try
                    {
                        AccountDetails AD = new AccountDetails();
                        AD.AccountID = acc.ID;
                        AD.AccountGUID = acc.AccountGUID;
                        AD.AccountName = acc.AccountName;
                        AD.DateCreated = "Coming Soon...";
                        AD.Email = CCUserRepository.Users.FirstOrDefault(u => u.AccountID == acc.ID).Email;
                        AD.Usgae = "Coming Soon...";
                        model.listOfAccounts.Add(AD);
                    }
                    catch (Exception ex) { }
                    
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult ImpersonatedAccount(long AccID)
        {

            if (Session["SysAdminDetails"] != null)
            {
                TempData["SelectedMenu"] = "AccImpersontation";

                var userObj = CCUserRepository.GetUserByEmailAddress(CCUserRepository.Users.First(U => U.AccountID == AccID).Email);
                Session["user"] = userObj;
                var accountObj = CCAccRepository.Accounts.FirstOrDefault(x => x.ID == userObj.AccountID);
                Session["account"] = accountObj;
                var timeZone = CCAccRepository.Accounts.Where(aid => aid.ID == userObj.AccountID).FirstOrDefault().TimeZone;
                Session["timeZone"] = timeZone;
                var folders = CCFolderRepository.CCFolders.Where(guid => guid.AccountGUID == accountObj.AccountGUID).ToList();
                Session["folderss"] = folders;

                LimitationsViewModel limitationsObj = new LimitationsViewModel();
                HelperFunctions HF1 = new HelperFunctions();
                limitationsObj = HF1.updateAccountLimitations(accountObj);
                Session["limitations"] = limitationsObj;

                List<NotificationListViewModel> notificationList = new List<NotificationListViewModel>();

                CronJobController CJC = new CronJobController();
                Session["account"] = CJC.checkAccountTrialExpiryForAccount(accountObj);

                HelperFunctions HF = new HelperFunctions();
                notificationList = HF.generateNotificationList(accountObj);

                HF.CheckAcccountStatus(accountObj);

                if (notificationList.Count > 0)
                    Session["notifications"] = notificationList;
                else
                    Session["notifications"] = null;

                if (((DateTime)(accountObj.TrialEnds) - (DateTime)(DateTime.Now.Date)).TotalDays < CCGlobalValues.trialPeriod)
                {
                    TrialDataModel trialObj = new TrialDataModel();
                    trialObj.hasPurchased = accountObj.HasPurchased;
                    trialObj.createdDate = accountObj.CreatedDate;
                    trialObj.trialEndDate = accountObj.TrialEnds;
                    trialObj.showPurchaseOptions = true;
                    Session["trialData"] = trialObj;
                }

                ImpersonateAccountsSysAdminModel model = new ImpersonateAccountsSysAdminModel();

                var AccountsInfo = CCAccRepository.Accounts.Where(A => A.ID == AccID).FirstOrDefault();

                model.SelectedAccountInfo = AccountsInfo;
                model.FolderCount = CCFolderRepository.CCFolders.Where(F => F.AccountGUID == AccountsInfo.AccountGUID).Count();
                model.ConnectionCount = CCConnectionRepository.CCSubscriptions.Where(C => C.AccountGUID == AccountsInfo.AccountGUID).Count();
                model.UserCount = CCUserRepository.Users.Where(U => U.AccountID == AccountsInfo.ID).Count();
                model.SubscriptionCount = 0;
                model.ItemsCount = CCItemRepository.CCContacts.Where(I => I.AccountGUID == AccountsInfo.AccountGUID).Count();
                model.SelectedAccountOwner = CCUserRepository.Users.First(U => U.AccountID == AccountsInfo.ID).FullName;

                //Sync Graph Data

                List<string> SyncDates = new List<string>();
                List<double> SyncDateUsage = new List<double>();

                DateTime startDate = DateTime.Now.Date.AddDays(-1);

                for (int i = 6; i > -1; i--)
                {
                    SyncDates.Add((startDate.AddDays(-i)).ToShortDateString());
                }

                for (int i = 0; i < SyncDates.Count(); i++)
                {
                    DateTime syncDateStart = DateTime.Parse(SyncDates[i]);
                    DateTime syncDateEnd = syncDateStart.AddHours(23).AddMinutes(59);

                    SyncDateUsage.Add(CCSyncItemsRepository.CCSyncItems.Where(SI => SI.LastUpdated >= syncDateStart && SI.LastUpdated <= syncDateEnd).Count());
                }

                model.SyncDates = SyncDates;
                model.SyncDateUsage = SyncDateUsage;

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult ImpersonatedAccountFolderListView(long AccID)
        {
            //TempData["SelectedMenu"] = "AccImpersontation";
            ImpersonateAccountsSysAdminModel model = new ImpersonateAccountsSysAdminModel();
            var AccountsInfo = CCAccRepository.Accounts.Where(A => A.ID == AccID).FirstOrDefault();

            model.SelectedAccountInfo = AccountsInfo;

            model.SelectedAccountFolderList = new List<FolderListDetails>();

            var folderList = CCFolderRepository.CCFolders.Where(F => F.AccountGUID == AccountsInfo.AccountGUID).ToList();

            foreach(var fold in folderList)
            {
                FolderListDetails FLD = new FolderListDetails();
                FLD.FolderDetails = fold;
                FLD.ItemCount = CCItemRepository.CCContacts.Where(I => I.FolderID == fold.FolderID).Count();
                model.SelectedAccountFolderList.Add(FLD);
            }

            return View(model);
        }

        public ActionResult ImpersonatedAccountFolderItemsView(long FoldID)
        {
            return RedirectToAction("Items", "Folder", new { id = FoldID });
        }

        public ActionResult ImpersonatedAccountConnectionListView(long AccID)
        {
            TempData["SelectedMenu"] = "AccImpersontation";
            ImpersonateAccountsSysAdminModel model = new ImpersonateAccountsSysAdminModel();

            return View(model);
        }

        public ActionResult SystemStatus()
        {
            TempData["SelectedMenu"] = "SystemStatus";

            HealthMsgsViewModel healthMsgObj = new HealthMsgsViewModel();
            healthMsgObj.HealthMsgList = CCHealthMsgsRepository.CCHealthMsgs.OrderByDescending(HM=>HM.ID).ToList();

            return View(healthMsgObj);
        }

        [HttpPost]
        public ActionResult SystemStatus(HealthMsgsViewModel healthMsgObj)
        {

            CCHealthMsgs healthMsgCurObj = healthMsgObj.newObj;

            healthMsgCurObj.IssueDateTime = DateTime.Now;
            healthMsgCurObj.FixDateTime = DateTime.Parse(healthMsgObj.FixDateTimeString);
            healthMsgCurObj = CCHealthMsgsRepository.SaveHealthMsg(healthMsgCurObj);

            return RedirectToAction("SystemStatus", "CorporateContactsAdmin");
        }
    }
}