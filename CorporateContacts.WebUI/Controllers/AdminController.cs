using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;
using Xobnu.WebUI.Infrastructure;
using Xobnu.WebUI.Models;
using Stripe;
using Newtonsoft.Json;
using Xobnu.Domain.Concrete;
using System.Configuration;
using Xobnu.WebUI.Util;
using System.Web.Security;

namespace Xobnu.WebUI.Controllers
{
    [Authorize]
    [CheckSessionOutAttribute]
    [CheckUserIsAdmin]
    [HandleError]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        IAuthProvider authProvider;
        IAccountRepo accountRepo;
        IPlanRepo planRepository;
        IUserRepo userRepository;
        IPurchasedFeatureRepo purchRepository;
        INotificationManager notifManager;
        IFolderRepo folderRepo;
        IFeatureRepo featureRepository;
        ICCFolderRepo CCFolderRepository;
        ICCItemRepo CCContactRepository;
        ICCConnectionsRepo CCConnectionRepository;
        ICCErrorLogRepo CCErrorLogRepository;
        IUserRepo CCUserRepository;
        ICCItemRepo CCItemRepository;
        ICCHealthMsgs CCHealthMsgsRepository;
        ICCFolderFieldRepo CCFieldRepository;
        ICCGroupRepo CCGroupRepository;
        ICCGroupFieldRepo CCGroupFieldRepository;

        private string rand = "00062008-0000-0000-C000-000000000046";

    //    int totalconnections;
        public AdminController(IAuthProvider auth, IAccountRepo accRepo, ICCGroupRepo group, ICCGroupFieldRepo groupfield, IUserRepo urep, ICCFolderFieldRepo field, IPlanRepo pRepo, IUserRepo user, ICCItemRepo items, IPurchasedFeatureRepo purRepo, INotificationManager notifMgr, IFolderRepo folderRep, IFeatureRepo featureRep, ICCFolderRepo ccfolder, ICCItemRepo contacts, ICCConnectionsRepo subscription, ICCErrorLogRepo errorlogs, ICCHealthMsgs healthMsgs)
        {
            authProvider = auth;
            userRepository = urep;
            planRepository = pRepo;
            accountRepo = accRepo;
            CCFieldRepository = field;
            purchRepository = purRepo;
            notifManager = notifMgr;
            folderRepo = folderRep;
            featureRepository = featureRep;
            CCFolderRepository = ccfolder;
            CCContactRepository = contacts;
            CCConnectionRepository = subscription;
            CCErrorLogRepository = errorlogs;
            CCUserRepository = user;
            CCItemRepository = items;
            CCHealthMsgsRepository = healthMsgs;
            CCGroupRepository = group;
            CCGroupFieldRepository = groupfield;

        }

        public ActionResult Index()
        {
            TempData["SelectedMenu"] = "Summary";
            TempData["SelectedSubMenu"] = "";
            TempData["SelectedSubMenuFolder"] = "";
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];
            if (userObj != null)
            {
                //if (accountObj.isOverFlow == true)
                //{
                //    return RedirectToAction("BillingOptions", "Admin", new { id = 1 });
                //}                
                
                LimitationsViewModel limitationsObj = new LimitationsViewModel();
                HelperFunctions HF = new HelperFunctions();
                limitationsObj = HF.updateAccountLimitations(accountObj);
                Session["limitations"] = limitationsObj;

                if (userObj.isPasswordChange == false)
                {
                    return RedirectToAction("ChangePassword", "Admin");
                }

                if (Session["trialData"] == null)
                { 
                    if (accountObj.HasPurchased == false)
                    {
                        if ((((DateTime)(accountObj.TrialEnds) - (DateTime)(DateTime.Now.Date)).TotalDays > (CCGlobalValues.trialPeriod - 2)) | (((DateTime)(accountObj.TrialEnds) - (DateTime)(DateTime.Now.Date)).TotalDays < 2))
                        { 
                            TrialDataModel trialObj = new TrialDataModel();
                            trialObj.hasPurchased = accountObj.HasPurchased;
                            trialObj.createdDate = accountObj.CreatedDate;
                            trialObj.trialEndDate = accountObj.TrialEnds;
                            trialObj.showPurchaseOptions = true;
                            Session["trialData"] = trialObj;

                            return RedirectToAction("Index", "Admin");
                        }
                        else if (((DateTime)(accountObj.TrialEnds) - (DateTime)(DateTime.Now.Date)).TotalDays < CCGlobalValues.trialPeriod)
                        {
                            TrialDataModel trialObj = new TrialDataModel();
                            trialObj.hasPurchased = accountObj.HasPurchased;
                            trialObj.createdDate = accountObj.CreatedDate;
                            trialObj.trialEndDate = accountObj.TrialEnds;
                            trialObj.showPurchaseOptions = true;
                            Session["trialData"] = trialObj;
                        }
                    }
                }

                if (accountObj.HasPurchased == true & Session["trialData"] != null)
                {
                    Session["trialData"] = null;
                }

                
                AdminViewModel ObjModel = new AdminViewModel();
                List<AdminDashboardViewModel> objDash = new List<AdminDashboardViewModel>();
                List<List<ConnectionInforViewModel>> connectionInforByConnction = new List<List<ConnectionInforViewModel>>();

                if (CCFolderRepository.CCFolders != null) { }
                var allFolders = CCFolderRepository.CCFolders.Where(guid => guid.AccountGUID == accountObj.AccountGUID).ToList();

                ObjModel.noOfFolders = CCFolderRepository.CCFolders.Where(F => F.AccountGUID == accountObj.AccountGUID).Count();
                ObjModel.noOfConnections = CCConnectionRepository.CCSubscriptions.Where(C => C.AccountGUID == accountObj.AccountGUID).Count();
                ObjModel.noOfUsers = CCUserRepository.Users.Where(U => U.AccountID == userObj.AccountID).Count();
                //ObjModel.noOfSubscriptionsPurchased = 0;
                ObjModel.noOfTotalItems = CCItemRepository.CCContacts.Where(I => I.AccountGUID == accountObj.AccountGUID & I.isDeleted == false).Count();

                

                // get saved quantity
                var accDetails = accountRepo.Accounts.Where(aguid => aguid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();
                var planLeval = planRepository.Plans.Where(pid => pid.ID == accDetails.PlanID).FirstOrDefault().PlanLevel;
                
                    var featureQuality = featureRepository.Features.Where(pid => pid.PlanLevel == planLeval & pid.Type == "Max Items per Folder").FirstOrDefault();
                    var savedQuality = purchRepository.Purchases.Where(fid => fid.FeatureID == featureQuality.ID && fid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();

                    if (savedQuality != null)
                    {
                        var quantitySaved = (savedQuality.Quantity) / (featureQuality.Quantity);
                        ObjModel.NoOfConnection = quantitySaved * 5;
                        ObjModel.noOfSubscriptionsPurchased = quantitySaved;
                        //limitationsObj.availableCconnectionCount = (int)ObjModel.NoOfConnection - (int)ObjModel.noOfConnections;
                        //limitationsObj.maxItemCountPerFolder = featureQuality.Quantity;
                        //if (featureRepository.Features.Where(pid => pid.PlanLevel == planLeval & pid.Type == "Sync Calendar").FirstOrDefault().Quantity == 0)
                        //    limitationsObj.isCalendarSyncAvailable = false;
                        //else
                        //    limitationsObj.isCalendarSyncAvailable = true;
                    }
                    else
                    {
                        ObjModel.NoOfConnection = 0;
                        ObjModel.noOfSubscriptionsPurchased = 0;
                    }
                 


                foreach (var folder in allFolders)
                {
                    AdminDashboardViewModel dash = new AdminDashboardViewModel();
                    List<ConnectionInforViewModel> connectionInforByFolder = new List<ConnectionInforViewModel>();

                    dash.FolderName = folder.Name;
                    dash.FolderID = folder.FolderID;
                    dash.FolderType = folder.Type;
                    dash.NumberOfItems = CCContactRepository.CCContacts.Where(fid => fid.FolderID == folder.FolderID & fid.AccountGUID == accountObj.AccountGUID & fid.isDeleted == false).Count();
                    dash.NumberOfConnections = CCConnectionRepository.CCSubscriptions.Where(fid => fid.FolderID == folder.FolderID & fid.AccountGUID == accountObj.AccountGUID).Count();

                                     
                    
                    if (folder.IsPaused == false)
                    {
                        dash.IsPaused = false;
                    }
                    else
                    {
                        dash.IsPaused = true;
                    }
                    objDash.Add(dash);

                    var connections = CCConnectionRepository.CCSubscriptions.Where(fid => fid.FolderID == folder.FolderID & fid.AccountGUID == accountObj.AccountGUID);
                    foreach (var conn in connections)
                    {
                        ConnectionInforViewModel connectionInfor = new ConnectionInforViewModel();
                        connectionInfor.FolderName = folder.Name;
                        connectionInfor.ConnctionFolderName = conn.FolderName;
                        connectionInfor.ConnectionID = conn.ConnectionID;

                        if (DateTime.Parse(conn.LastSyncTime) < DateTime.Parse("1902-01-01 00:00"))
                        {
                            connectionInfor.LastSyncTime = "Never";
                        }
                        else
                        {

                            DateTime timeUtc = DateTime.Parse(conn.LastSyncTime);
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

                            connectionInfor.LastSyncTime = cstTime.ToString();



                            //connectionInfor.LastSyncTime = conn.LastSyncTime;
                        }
                        connectionInfor.Type = conn.Type;
                        connectionInfor.FolderID = folder.FolderID;
                        if (conn.IsRunning == null) { connectionInfor.IsRunning = false; }
                        else if (conn.IsRunning == false) { connectionInfor.IsRunning = false; }
                        else if (conn.IsRunning == true) { connectionInfor.IsRunning = true; }
                        if (conn.IsPaused == false)
                        {
                            connectionInfor.IsPaused = false;
                            connectionInfor.rowClass = "notPaused ";
                        }
                        else
                        {
                            connectionInfor.IsPaused = true;
                            connectionInfor.rowClass = "Paused ";
                        }
                        connectionInforByFolder.Add(connectionInfor);

                        

                    }

                    connectionInforByConnction.Add(connectionInforByFolder);

                }
                var assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                ViewBag.version = assemblyVersion;
                Session["version"] = assemblyVersion;
                ObjModel.FoldersInfor = objDash;
                ObjModel.ConnectionsInfor = connectionInforByConnction;
                return View(ObjModel);
            }
            return View();
        }

     
        [HttpPost]
        public ActionResult SendErrorNotification(string actionNmae, string controllerName, string messageExce)
        {
            User userObj = (User)Session["user"];
            if (userObj != null)
            {
                string userName = userObj.Email;
                string password = userObj.Password;

                // send Email Notification 
                var notifObj = new Notification()
                {
                    Subject = " Corporate contacts !",
                    RecipientAddress = "corp_errors@davton.com",
                    Message = "<html><head></head><body><p>Hello, \n\n</p><p>Exception message:-\n" + messageExce + "</p><p>ControllerName:- " + controllerName + "</P><p>ActionName:- " + actionNmae + "</p><p>UserName:- " + userName + "</p><p>UserID:- " + userObj.ID + "</p><p>AccountID:- " + userObj.AccountID + "</p><p>Host Name:- " + Request.Url.GetLeftPart(UriPartial.Authority) + "</p><p>User Type:- " + userObj.UserType + "</p></body></html>"
                };

                notifManager.SendNotification(notifObj);
            }
            else
            {
                var notifObj = new Notification()
                {
                    Subject = " Corporate contacts !",
                    RecipientAddress = "corp_errors@davton.com",
                    Message = "Hello, \n\n" + "Error on creating account \n\n Exception message:-\n" + messageExce
                };
                notifManager.SendNotification(notifObj);
            }
            return View();
        }

     
                
        public ActionResult AccountOptions()
        {

            TempData["SelectedMenu"] = "Account";
            TempData["SelectedSubMenu"] = "Options";
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];


            if (userObj.PrimaryUser == false)
                return RedirectToAction("Index", "Admin");   

            AccountInfomationViewModel accountinfor = new AccountInfomationViewModel();

            //get time zone 
            System.Collections.ObjectModel.ReadOnlyCollection<TimeZoneInfo> tz;
            tz = TimeZoneInfo.GetSystemTimeZones();
            List<TimeZoneViewModel> tzList = new List<TimeZoneViewModel>();

            foreach (var t in tz)
            {
                TimeZoneViewModel tzvmodel = new TimeZoneViewModel();
                tzvmodel.DisplayName = t.DisplayName;
                tzvmodel.ID = t.Id;
                tzList.Add(tzvmodel);
            }

            accountinfor.TimeZoneList = tzList;

            if (userObj != null)
            {
                var account = accountRepo.Accounts.Where(aid => aid.ID == userObj.AccountID).FirstOrDefault();
                accountinfor.AccountName = account.AccountName;
                accountinfor.ContactName = userObj.FullName;
                accountinfor.EmailAddress = userObj.Email;
                accountinfor.TimeZone = account.TimeZone;
                accountinfor.BillingAddress = account.BusinessAddress;

                //get numberof connection 
                int planid = account.PlanID;
                int planLevel = planid * 10;

                //get billing plan 
                var billingPlan = planRepository.Plans.Where(pid => pid.ID == planid).FirstOrDefault();
                accountinfor.BillingPlan = billingPlan.Name;

                // get saved quantity
                var accDetails = accountRepo.Accounts.Where(aguid => aguid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();
                var planLeval = planRepository.Plans.Where(pid => pid.ID == accDetails.PlanID).FirstOrDefault().PlanLevel;

                var featureQuality = featureRepository.Features.Where(pid => pid.PlanLevel == planLeval & pid.Type == "Max Items per Folder").FirstOrDefault();
                    var savedQuality = purchRepository.Purchases.Where(fid => fid.FeatureID == featureQuality.ID && fid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();

                    if (savedQuality != null)
                    {
                        var quantitySaved = (savedQuality.Quantity) / (featureQuality.Quantity);
                        accountinfor.NoOfConnection = quantitySaved * 5;

                    }
                    else
                    {
                        accountinfor.NoOfConnection = 0;
                    }
                

                accountinfor.ListofUsers = new List<Domain.Entities.User>();
                accountinfor.ListofUsers = userRepository.Users.Where(u => u.AccountID == userObj.AccountID).ToList();

                //var errorLogsObj = CCErrorLogRepository.CCErrorLogs.Where(guid => guid.AccountGUID == accountObj.AccountGUID & (guid.ErrorType == "Exchange Connection" | guid.ErrorType == "Sync")).Take(100).ToList();
                accountinfor.ErrorLogList = new List<CCErrorLog>();
                //accountinfor.ErrorLogList = errorLogsObj;


                var plans = planRepository.Plans.ToList();
                // plans.RemoveAll(pname => pname.Name == "Free");
                accountinfor.Plans = plans;

                // get selected plan            
                var planID = accountRepo.Accounts.FirstOrDefault(aguid => aguid.AccountGUID == accountObj.AccountGUID).PlanID;
                accountinfor.PlanID = planID;

                // get selected plan details 
                var selectedPlanDetails = plans.FirstOrDefault(pid => pid.ID == planID);
                selectedPlanDetails.Price = selectedPlanDetails.Price;
                //packageViewModel.SelectedPlanDetails = selectedPlanDetails;

                // get card details from stripe
                var stripeCustomerID = accountRepo.Accounts.FirstOrDefault(aguid => aguid.AccountGUID == accountObj.AccountGUID).StripeCustomerID;

                if (stripeCustomerID != null)
                {
                    // default card details 
                    var customer = new StripeCustomerService();
           //         customer.ApiKey = "sk_test_4Xusc3Meo8gniONh6dDRZvlp";
                    var cusRes = customer.Get(stripeCustomerID);
                    var defaultCardID = cusRes.StripeDefaultCardId;
                    //packageViewModel.DefaultCardID = defaultCardID;

                    // get saved quantity
                    featureQuality = featureRepository.Features.FirstOrDefault(pid => pid.PlanLevel == selectedPlanDetails.PlanLevel & pid.Type == "Max Items per Folder");
                    savedQuality = purchRepository.Purchases.FirstOrDefault(fid => fid.FeatureID == featureQuality.ID && fid.AccountGUID == accountObj.AccountGUID);

                    if (savedQuality != null)
                    {
                        var quantitySaved = (savedQuality.Quantity) / (featureQuality.Quantity);
                        accountinfor.QuantitySaved = quantitySaved;

                    }
                    else
                    {
                        accountinfor.QuantitySaved = 1;
                    }

                    var cards = new StripeCardService();
                //    cards.ApiKey = "sk_test_4Xusc3Meo8gniONh6dDRZvlp";
                    var objCard = cards.List(stripeCustomerID, null);
                    List<CardViewModel> allCardDetails = new List<CardViewModel>();

                    foreach (var card in objCard)
                    {
                        CardViewModel objcard = new CardViewModel();
                        objcard.CardID = card.Id;
                        objcard.Name = card.Name;
                        objcard.Number = "**** " + card.Last4;
                        objcard.Type = card.Brand;
                        objcard.ExpireDate = card.ExpirationYear + "/" + card.ExpirationMonth;

                        allCardDetails.Add(objcard);
                    }
                    //packageViewModel.CardDetails = allCardDetails;
                }
                else
                {
                    //packageViewModel.CardDetails = null;
                }

            }

            return View(accountinfor);
        }


       

        public ActionResult exsistingStripeCustomer()
        {
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];

            var currentAccountDetails = accountRepo.Accounts.FirstOrDefault(aid => aid.ID == userObj.AccountID);

            if (currentAccountDetails.StripeCustomerID != null)
            {
                return Json("Exsisitng", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("NewUser", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ChangePassword()
        {
            User LoggedInUserObj = (User)Session["user"];

            return View(LoggedInUserObj);
        }

        [HttpPost]
        public ActionResult ChangePassword(User userObj)
        {
            User LoggedInUserObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];

            //var userToEdit = CCUserRepository.Users.Where(u => u.ID == userObj.ID).FirstOrDefault();

            //var test = Encryption.DecryptStringAES(LoggedInUserObj.Password, rand);

            if (Encryption.DecryptStringAES(LoggedInUserObj.Password, rand) == userObj.CurrentPassword)
            {

                if (userObj.Password != userObj.ConfirmPassword)
                {
                    Session["NewUserInfo"] = "Make sure you enter the New Passwords in both places correctly";
                    return RedirectToAction("ChangePassword");
                }
                else {
                    LoggedInUserObj.Password = Encryption.EncryptStringAES(userObj.Password, rand);
                    LoggedInUserObj.isPasswordChange = true;
                    var user = userRepository.SaveUser(LoggedInUserObj);
                    Session["user"] = user;
                    Session["NewUserInfo"] = "User Password Changed Successfully";
                    return RedirectToAction("AccountOptions");
                }
            }
            else
            {
                Session["NewUserInfo"] = "Entered Current Password does not match Users Current Password";
                return RedirectToAction("ChangePassword");
            }

        }

     

        public FileResult GetFile()
    {
        string fileName = "CorporateContactsSetupInstructions.pdf";
     
       string path = AppDomain.CurrentDomain.BaseDirectory + "App_Data/";            
       return File(path + fileName, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
}


    }
}
