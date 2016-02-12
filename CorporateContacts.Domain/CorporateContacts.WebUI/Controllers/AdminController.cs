using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;
using CorporateContacts.WebUI.Infrastructure;
using CorporateContacts.WebUI.Models;
using Stripe;
using Newtonsoft.Json;
using CorporateContacts.Domain.Concrete;
using System.Configuration;
using CorporateContacts.WebUI.Util;
using System.Web.Security;

namespace CorporateContacts.WebUI.Controllers
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

        public ActionResult SupportTicket()
        {
            TempData["SelectedMenu"] = "Support";
            return View();
        }

        public ActionResult ChangeSyncState(string ActionType, string ID, string Value)
        {
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];

            if (ModelState.IsValid && accountObj != null)
            {
                if (ActionType == "Folder")
                {
                    CCFolder folderObj = new CCFolder();
                    folderObj.FolderID = long.Parse(ID);

                    if (Value == "1")
                        folderObj.IsPaused = false;
                    else
                        folderObj.IsPaused = true;

                    bool folderUpdate = CCFolderRepository.UpdatePauseSync(folderObj);

                    //Change state of IsPaused in Connections as well..
                    var connectionListOfFolder = CCConnectionRepository.CCSubscriptions.Where(fid => fid.FolderID == folderObj.FolderID).ToList();

                    foreach (var conn in connectionListOfFolder)
                    {
                        CCConnection connectionObj = new CCConnection();
                        connectionObj.ConnectionID = conn.ConnectionID;

                        if (Value == "1")
                            connectionObj.IsPaused = false;
                        else
                            connectionObj.IsPaused = true;

                        bool connectionUpdate = CCConnectionRepository.UpdatePauseSync(connectionObj);
                    }
                }
                else if (ActionType == "Connection")
                {
                    CCConnection connectionObj = new CCConnection();
                    connectionObj.ConnectionID = long.Parse(ID);

                    if (Value == "1")
                        connectionObj.IsPaused = false;
                    else
                        connectionObj.IsPaused = true;

                    bool connectionUpdate = CCConnectionRepository.UpdatePauseSync(connectionObj);
                }
            }
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult Users()
        {
            TempData["SelectedMenu"] = "Account";
            User userObj = (User)Session["user"];
            TempData["SelectedMenu"] = "Users";
            return View(userRepository.Users.Where(u => u.AccountID == userObj.AccountID));
        }

        public ActionResult EditUser(long ID)
        {
            TempData["SelectedMenu"] = "Account";
            TempData["SelectedSubMenu"] = "Options";
            User userObj = (User)Session["user"];
            if (userObj.PrimaryUser == false)
                return RedirectToAction("Index", "Admin");  

            var myAccountUsers = userRepository.Users.Where(u => u.AccountID == userObj.AccountID);
            var userToEdit = myAccountUsers.FirstOrDefault(u => u.ID == ID);
            if (userToEdit == null)
            {
                throw new ArgumentNullException("No such user");
            }
            else return View(userToEdit);
        }

        public ActionResult NewUser()
        {
            TempData["SelectedMenu"] = "Account";
            TempData["SelectedSubMenu"] = "Options";
            ViewBag.SelectedMenu = "Users";
            User userObj = (User)Session["user"];

            if (userObj.PrimaryUser == false)
                return RedirectToAction("Index", "Admin");   

            return View();
        }       

        [HttpPost]
        public ActionResult EditUser(User userObj)
        {
            if (ModelState.IsValid)
            {
                User currentUserObj = (User)Session["user"];
                Account accountObj = (Account)Session["account"];
                userObj.AccountID = currentUserObj.AccountID;

                var userExsist = CCUserRepository.Users.Where(u => u.Email == userObj.Email).Count();

                if (userObj.ID == 0 && userExsist == 0)
                {
                    //new user 
                    userObj.GUID = Guid.NewGuid().ToString();
                    userObj.Password = Membership.GeneratePassword(12, 1);

                    var notifObj = new Notification()
                    {
                        Subject = "Welcome to Corporate contacts !",
                        RecipientAddress = userObj.Email,
                        //RecipientAddress = "brendan@davtonuk.com",
                        // Message = "Hello, \n\n" + currentUserObj.FullName + " has invited you to join corporate contacts.\nClick the link below to join.\n\n" + Request.Url.GetLeftPart(UriPartial.Authority) + "/signup/NewUser?guid=" + userObj.GUID + "\n\nCorporate Contacts Team"
                        //Message = "Hello, \n\n" + currentUserObj.FullName + " has invited you to join corporate contacts. \nClick the link below to join.\n\n" + Request.Url.GetLeftPart(UriPartial.Authority) + "/VerifyUserAccount/VerifyEmailAddress?GUID=" + userObj.GUID + "\n\n" +
                        //"/n/n/Corporate Contacts Team."

                       // Message = "Hello, \n\n" + currentUserObj.FullName + " has invited you to join corporate contacts. \nClick the link below to join.\n\n" + Request.Url.GetLeftPart(UriPartial.Authority) + "/VerifyUserAccount/VerifyEmailAddress?GUID=" + userObj.GUID + "\n\n . Your credentials " +
                       //" for the system are as follows. /n/n Username : " + userObj.Email + "/n/n/ Password : " + userObj.Password + "/n/n/Corporate Contacts Team"

                        Message = "<html><head></head><body><div><center> <table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> </tr><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td align='center' valign='top' width='600' style='padding-left:0;padding-right:0;padding-top:0;padding-bottom:0;border-top:1px solid #cccccc;border-right:1px solid #cccccc;border-left:1px solid #cccccc;background-color:#ffffff'><img src='http://corporate-contacts.com/wp-content/uploads/2014/03/CorporateContactslogo3a2.png' style='max-width: 450px;margin-top: 20px;' alt='Dyn'></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc;background-color:#ffffff'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color:#ffffff'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='top' style='padding-top:20px;padding-right:30px;padding-left:30px'><table align='left' border='0' cellpadding='0' cellspacing='0'> <tbody><tr> <td valign='top' style='color:#444444;font-family:Helvetica;font-size:16px;line-height:125%;text-align:left;padding-bottom:20px'><p>Dear " + userObj.FullName + "</p><p> '" + accountObj.AccountName + "' has created an account for you in Corporate Contacts, a leading online contact and appointment management software.</p><p>Shown below are the credentials to the system. But first you need Click on the link below to join Corporate Contacts.<a href='" + Request.Url.GetLeftPart(UriPartial.Authority) + "/VerifyUserAccount/VerifyEmailAddress?GUID=" + userObj.GUID + "'>Click Here</a></p><p><a style='text-decoration:underline;color:#0066cc' href='" + Request.Url.GetLeftPart(UriPartial.Authority) + "/VerifyUserAccount/VerifyEmailAddress?GUID=" + userObj.GUID + "' target='_blank'>" + Request.Url.GetLeftPart(UriPartial.Authority) + "/VerifyUserAccount/VerifyEmailAddress?GUID=" + userObj.GUID + "</a></p><p><strong>Username :</strong> " + userObj.Email + "</p><p><strong>Password:</strong> " + userObj.Password + "</p><p>If you need help, you can find it here: <a href='http://support.corporate-contacts.com'>support.corporate-contacts.com</a></p><p>Regards,<br><a style='text-decoration:underline;color:#0066cc' href='http://corporate-contacts.com/' target='_blank'>The Corporate Contacts team</a></p></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color: #209FD1;'> <tbody><tr> <td align='center' valign='bottom'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td valign='bottom' width='600' style='padding-top:0px;padding-right:20px;padding-left:20px;padding-bottom:10px'><table align='left' border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Corporate Contacts Ltd.<br>Beverley, UK, HU17 0LD<br>Manchester, NH 03101</td><td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Phone: +44 1482 869700<br>E-Mail: hello@corporate-contacts.com</td></tr></tbody></table> </td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></center></div></body></html>"

                    };
                    notifManager.SendNotification(notifObj);

                    userObj.Status = "Invite Sent";    
                    if(userObj.isAdmin)
                        userObj.UserType = "Admin";
                    else
                        userObj.UserType = "Standard";
                    userObj.PrimaryUser = false;
                    userObj.CreatedDate = DateTime.Now;
                    userObj.LastModifiedDate = DateTime.Now;
                    userObj.Password = Encryption.EncryptStringAES(userObj.Password, rand);
                    userObj.isPasswordChange = false;
                    userObj.ConfirmPassword = userObj.Password;
                    var user = userRepository.SaveUser(userObj);
                    Session["NewUserInfo"] = "New User Added Successfully";
                    return RedirectToAction("AccountOptions");
                }
                else
                {
                    Session["NewUserInfo"] = "The Email address is already a User";
                    return RedirectToAction("NewUser");
                }
            }
            else
            {
                return View(userObj);
            }
        }

        public ActionResult DeleteUser(long ID)
        {
            User userObj = (User)Session["user"];
            var myAccountUsers = userRepository.Users.Where(u => u.AccountID == userObj.AccountID);
            var userToDelete = myAccountUsers.FirstOrDefault(u => u.ID == ID);
            if (userToDelete != null)
            {
                bool result = userRepository.DeleteUser(ID);
            }
            else
            {

            }
            return RedirectToAction("Users");
        }

        public ActionResult Subscription(int id)
        {
            TempData["SelectedMenu"] = "Account";
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];

            decimal additionalPrice = 0;
            //TempData["SelectedMenu"] = "Subscription";
            List<List<string>> typeListObj = new List<List<string>>();

            PackageViewModel packageViewModel = new PackageViewModel();

            //get all plans
            var plans = planRepository.Plans.ToList();
            // plans.RemoveAll(pname => pname.Name == "Free");
            packageViewModel.Plans = plans;

            // get all types
            //var types = featureRepository.Features.Where(p => p.Price == 0).Select(f => f.Type).Distinct().ToList();
            List<string> types = new List<string>();
            types.Add("Sync Contacts");
            types.Add("Sync Calendar");
            //types.Add("Contacts Sync");
            //types.Add("Calendar Sync");
            //types.Add("Connections");
            types.Add("Max Items per Folder");
            types.Add("Fields Available");
            types.Add("History Tracking");
            types.Add("Third Party Connections");
            packageViewModel.Types = types;

            // get selected plan            
            var planID = accountRepo.Accounts.FirstOrDefault(aguid => aguid.AccountGUID == accountObj.AccountGUID).PlanID;
            packageViewModel.PlanID = planID;

            // get selected plan details 
            var selectedPlanDetails = plans.FirstOrDefault(pid => pid.ID == planID);
            selectedPlanDetails.Price = selectedPlanDetails.Price;
            packageViewModel.SelectedPlanDetails = selectedPlanDetails;


            // run all plans
            foreach (var pln in plans)
            {

                var allFeatures = featureRepository.Features.Where(pl => pl.PlanLevel == pln.PlanLevel).ToList();

                List<string> typesList = new List<string>();

                foreach (var typ in types)
                {

                    var res = allFeatures.FirstOrDefault(t => t.Type == typ);

                    if (res != null)
                    {
                        typesList.Add(res.Name);
                    }
                    else
                    {
                        typesList.Add("-");
                    }

                }

                typeListObj.Add(typesList);

            }

            packageViewModel.Names = typeListObj;

            // get card details from stripe
            var stripeCustomerID = accountRepo.Accounts.FirstOrDefault(aguid => aguid.AccountGUID == accountObj.AccountGUID).StripeCustomerID;

            if (stripeCustomerID != null)
            {
                // default card details 
                var customer = new StripeCustomerService();
                customer.ApiKey = "sk_test_4Xusc3Meo8gniONh6dDRZvlp";
                var cusRes = customer.Get(stripeCustomerID);
                var defaultCardID = cusRes.StripeDefaultCardId;
                packageViewModel.DefaultCardID = defaultCardID;

                // get saved quantity
                var featureQuality = featureRepository.Features.FirstOrDefault(pid => pid.PlanLevel == selectedPlanDetails.PlanLevel);
                var savedQuality = purchRepository.Purchases.FirstOrDefault(fid => fid.FeatureID == featureQuality.ID && fid.AccountGUID == accountObj.AccountGUID);

                if (savedQuality != null)
                {
                    var quantitySaved = (savedQuality.Quantity) / (featureQuality.Quantity);
                    packageViewModel.QuantitySaved = quantitySaved;

                }
                else
                {
                    packageViewModel.QuantitySaved = 1;
                }

                var cards = new StripeCardService();
                cards.ApiKey = "sk_test_4Xusc3Meo8gniONh6dDRZvlp";
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
                packageViewModel.CardDetails = allCardDetails;
            }

            else
            {
                packageViewModel.CardDetails = null;
            }


            // End get card details 

            if (id == 2)
            {
                ViewBag.MessagePass = "Your package has successfully changed";
            }
            else
            {
                ViewBag.MessagePass = "";
            }

            return View(packageViewModel);
        }

        public ActionResult SetupSync(long primarySourceID = 0)
        {
            User userObj = (User)Session["user"];
            TempData["SelectedMenu"] = "SetupSync";
            var viewObj = new SetupSyncViewModel();
            viewObj.MasterSources = folderRepo.GetPrimarySourcesForAccount(userObj.AccountID);
            viewObj.SubscriptionsForCurrentMaster = folderRepo.GetSubscriptionForPrimarySource(primarySourceID, userObj.AccountID);
            viewObj.CurrentMasterID = primarySourceID;
            viewObj.CurrentMasterFolderName = folderRepo.GetFolderNameFromID(primarySourceID);
            return View(viewObj);
        }

        public ActionResult DeleteSub(long subid)
        {
            return RedirectToAction("Index");
        }

        public ActionResult Charge(string stripeToken, int quantities,string couponID)
        {
            decimal additionalPrice = 0;
            // get selected plan
            User userObj = (User)Session["user"];
            var planID = accountRepo.Accounts.FirstOrDefault(aid => aid.ID == userObj.AccountID).PlanID;

            // get selected plan details 
            var selectedPlanName = planRepository.Plans.FirstOrDefault(pid => pid.ID == planID);

            // create subscription and get subscription id
            var customerService = new StripeCustomerService();
            customerService.ApiKey = "sk_test_4Xusc3Meo8gniONh6dDRZvlp";

            var currentAccountDetails = accountRepo.Accounts.FirstOrDefault(aid => aid.ID == userObj.AccountID);
            
            var customer = new StripeCustomerCreateOptions();
            customer.PlanId = selectedPlanName.Name;
            customer.Quantity = quantities;
            customer.TokenId = stripeToken;
            customer.Email = userObj.Email;
            if (couponID != "")
            {
                customer.CouponId = couponID;
            }

            // Create subscription
            try
            {
                var subscriptionDetails = customerService.Create(customer);
                var stripeCustomerID = subscriptionDetails.Id;

                // save StripeCustomerID          
                currentAccountDetails.StripeCustomerID = stripeCustomerID;
                accountRepo.SaveAccount(currentAccountDetails);
                Session["account"] = currentAccountDetails;
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Session["paymentFailureNotifications"] = "card_declined";
                return RedirectToAction("BillingOptions", "Admin", new { id = 1 });
            }
            

            
            //return RedirectToAction("BillingOptions", "Admin", new { id = 1 });
        }

        public ActionResult upgradeTrialAccount(string plan)
        {
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];
            //int planLevel = Int32.Parse(plan);
            //int existingPlanID = 1;

            ////Delete old purchasedfeature
            //var purchasedfeatures = purchRepository.Purchases.Where(p => p.AccountGUID == accountObj.AccountGUID).ToList();
            //foreach (var purch in purchasedfeatures)
            //{
            //    bool res = purchRepository.DeleteFPurchasedFeature(purch.ID);

            //}

            //var featuresForPlan = featureRepository.Features.Where(pl => pl.PlanLevel == planLevel).ToList();

            //// save default feature 
            //var featuresForDefaultPlan = featureRepository.Features.Where(pl => pl.PlanLevel == planLevel && pl.Price == 0).ToList();

            //foreach (var feat in featuresForDefaultPlan)
            //{
            //    PurchasedFeatures objFeature = new PurchasedFeatures();
            //    objFeature.AccountGUID = accountObj.AccountGUID;
            //    objFeature.FeatureID = feat.ID;
            //    objFeature.ExpiryDate = DateTime.UtcNow;
            //    objFeature.Enabled = true;
            //    objFeature.Quantity = (feat.Quantity) * 1;
            //    purchRepository.SavePurchase(objFeature);
            //}

            //// change the plain id
            //var planid = planRepository.Plans.FirstOrDefault(pl => pl.PlanLevel == planLevel).ID;

            //var currentAccount = accountRepo.Accounts.FirstOrDefault(ac => ac.ID == userObj.AccountID);

            //Account account = new Account();
            //account = currentAccount;
            //existingPlanID = account.PlanID;
            //account.PlanID = planid;           
            //account = accountRepo.SaveAccount(account);

            var account = saveFeaturesCommon(plan, 1);

            Session["account"] = account;

            var currentAccountDetails = accountRepo.Accounts.FirstOrDefault(aid => aid.ID == userObj.AccountID);

            if (account != null)
            {                
               return Json("Sucess", JsonRequestBehavior.AllowGet); 
            }
            else
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult AdditionalFeatures(string plan)
        {
            Account accountObj = (Account)Session["account"];
            FeaturesViewModel featuresViewModel = new FeaturesViewModel();

            int planLevel = Int32.Parse(plan);


            User userObj = (User)Session["user"];

            var curPlanLevel = planRepository.Plans.FirstOrDefault(pid => pid.ID == accountObj.PlanID);
            var curFeatureQuantity = featureRepository.Features.FirstOrDefault(pid => pid.PlanLevel == curPlanLevel.PlanLevel & pid.Type == "Max Items per Folder");

            //var featureQuality = featureRepository.Features.FirstOrDefault(pid => pid.PlanLevel == planLevel & pid.Type == "Max Items per Folder");
            var savedQuality = purchRepository.Purchases.FirstOrDefault(fid => fid.FeatureID == curFeatureQuantity.ID && fid.AccountGUID == accountObj.AccountGUID);


            if (savedQuality != null)
            {
                var quantitySaved = (savedQuality.Quantity) / (curFeatureQuantity.Quantity);
                featuresViewModel.PurchasedQuantities = quantitySaved;
                return Json(featuresViewModel, JsonRequestBehavior.AllowGet);

            }
            else
            {

                return Json(1, JsonRequestBehavior.AllowGet);

            }


        }


        public Account saveFeaturesCommon(string plan, int quantities)
        {
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];
            int planLevel = Int32.Parse(plan);
            int existingPlanID = 1;


            //Delete old purchasedfeature
            var purchasedfeatures = purchRepository.Purchases.Where(p => p.AccountGUID == accountObj.AccountGUID).ToList();
            foreach (var purch in purchasedfeatures)
            {
                bool res = purchRepository.DeleteFPurchasedFeature(purch.ID);

            }

            var featuresForPlan = featureRepository.Features.Where(pl => pl.PlanLevel == planLevel).ToList();

            // save default feature 
            var featuresForDefaultPlan = featureRepository.Features.Where(pl => pl.PlanLevel == planLevel && pl.Price == 0).ToList();

            foreach (var feat in featuresForDefaultPlan)
            {
                PurchasedFeatures objFeature = new PurchasedFeatures();
                objFeature.AccountGUID = accountObj.AccountGUID;
                objFeature.FeatureID = feat.ID;
                objFeature.ExpiryDate = DateTime.UtcNow;
                objFeature.Enabled = true;

                if (feat.Type == "Max Items per Folder")
                {
                    objFeature.Quantity = (feat.Quantity) * quantities;
                }
                else
                {
                    objFeature.Quantity = feat.Quantity;
                }


                purchRepository.SavePurchase(objFeature);
            }

            //Ajust Folder Fields START

            List<string> needtosave = new List<string>();
            List<CCFolderField> savedobj = new List<CCFolderField>();
            List<CCFolderField> activeFields = new List<CCFolderField>();
            List<CCFolderField> needToInActive = new List<CCFolderField>();

            List<String> objFields = new List<string>();

            if (planLevel == 10)
                objFields = FieldsConfigHelper.GetFieldForContactSimple();
            else if (planLevel == 20)
                objFields = FieldsConfigHelper.GetFieldForContactBusiness();
            else
                objFields = FieldsConfigHelper.GetFieldForContactFull();


            var folderListForAccount = CCFolderRepository.CCFolders.Where(F => F.AccountGUID == accountObj.AccountGUID).ToList();

            foreach (var fold in folderListForAccount.Where(f=>f.Type==1))
            {
                needtosave = CCFieldRepository.IsAvailableField(objFields, fold.FolderID);

                foreach (var field in needtosave)
                {
                    CCFolderField folderField = new CCFolderField();
                    folderField.FieldName = FieldsConfigHelper.GetRealName(field);
                    folderField.FieldType = FieldsConfigHelper.GetVariableType(field);
                    folderField.FolderID = fold.FolderID;
                    folderField.FieldCaption = field;
                    folderField.Restriction = "none";
                    folderField.AccountGUID = accountObj.AccountGUID;
                    folderField.isActive = true;
                    savedobj.Add(folderField);
                }

                var resp = CCFieldRepository.SaveFolderFieldsObj(savedobj);

                foreach (var field in objFields)
                {
                    CCFolderField folderField = CCFieldRepository.CCFolderFields.Where(f => f.FieldCaption == field & f.AccountGUID == accountObj.AccountGUID & f.FolderID == fold.FolderID).FirstOrDefault();
                    activeFields.Add(folderField);
                }

                var fieldList = CCFieldRepository.CCFolderFields.Where(f => f.AccountGUID == accountObj.AccountGUID & f.FolderID == fold.FolderID).ToList();

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
                        { }

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
                var defaultGrp = CCGroupRepository.CCGroups.Where(fid => fid.FolderID == fold.FolderID & fid.GroupName == "Default" & fid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();

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
            }



            //Ajust Folder Fields END





            // change the plain id
            var planid = planRepository.Plans.FirstOrDefault(pl => pl.PlanLevel == planLevel).ID;

            var currentAccount = accountRepo.Accounts.FirstOrDefault(ac => ac.ID == userObj.AccountID);

            Account account = new Account();
            account = currentAccount;
            existingPlanID = account.PlanID;
            account.PlanID = planid;
            account.SyncPeriod = (short)featureRepository.Features.Where(pl => pl.PlanLevel == planLevel && pl.Type == "Sync Period").First().Quantity;
            account = accountRepo.SaveAccount(account);

            return account;
        }

        public ActionResult SaveFeatures(string plan, int quantities, string couponID)
        {
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];
            int planLevel = Int32.Parse(plan);
            int existingPlanID = accountObj.PlanID;
            string purchaseType = "Upgrade";



            ////Delete old purchasedfeature
            //var purchasedfeatures = purchRepository.Purchases.Where(p => p.AccountGUID == accountObj.AccountGUID).ToList();
            //foreach (var purch in purchasedfeatures)
            //{
            //    bool res = purchRepository.DeleteFPurchasedFeature(purch.ID);

            //}

            //var featuresForPlan = featureRepository.Features.Where(pl => pl.PlanLevel == planLevel).ToList();

            //// save default feature 
            //var featuresForDefaultPlan = featureRepository.Features.Where(pl => pl.PlanLevel == planLevel && pl.Price == 0).ToList();

            //foreach (var feat in featuresForDefaultPlan)
            //{
            //    PurchasedFeatures objFeature = new PurchasedFeatures();
            //    objFeature.AccountGUID = accountObj.AccountGUID;
            //    objFeature.FeatureID = feat.ID;
            //    objFeature.ExpiryDate = DateTime.UtcNow;
            //    objFeature.Enabled = true;

            //    if (feat.Type == "Max Items per Folder")
            //    {
            //        objFeature.Quantity = (feat.Quantity) * quantities;
            //    }
            //    else
            //    {
            //        objFeature.Quantity = feat.Quantity;
            //    }

                
            //    purchRepository.SavePurchase(objFeature);
            //}

            ////Ajust Folder Fields START

            //List<string> needtosave = new List<string>();
            //List<CCFolderField> savedobj = new List<CCFolderField>();
            //List<CCFolderField> activeFields = new List<CCFolderField>();
            //List<CCFolderField> needToInActive = new List<CCFolderField>();

            //List<String> objFields = new List<string>();

            //if(planLevel == 10)  
            //    objFields = FieldsConfigHelper.GetFieldForAppointmentSimple();
            //else if(planLevel == 20)
            //    objFields = FieldsConfigHelper.GetFieldForContactBusiness();
            //else
            //    objFields = FieldsConfigHelper.GetFieldForContactFull();


            //var folderListForAccount = CCFolderRepository.CCFolders.Where(F => F.AccountGUID == accountObj.AccountGUID).ToList();

            //foreach (var fold in folderListForAccount)
            //{
            //    needtosave = CCFieldRepository.IsAvailableField(objFields, fold.FolderID);

            //    foreach (var field in needtosave)
            //    {
            //        CCFolderField folderField = new CCFolderField();
            //        folderField.FieldName = FieldsConfigHelper.GetRealName(field);
            //        folderField.FieldType = FieldsConfigHelper.GetVariableType(field);
            //        folderField.FolderID = fold.FolderID;
            //        folderField.FieldCaption = field;
            //        folderField.Restriction = "none";
            //        folderField.AccountGUID = accountObj.AccountGUID;
            //        folderField.isActive = true;
            //        savedobj.Add(folderField);
            //    }

            //    foreach (var field in objFields)
            //    {
            //        CCFolderField folderField = CCFieldRepository.CCFolderFields.Where(f => f.FieldCaption == field & f.AccountGUID == accountObj.AccountGUID & f.FolderID == fold.FolderID).FirstOrDefault();
            //        activeFields.Add(folderField);
            //    }

            //    var resp = CCFieldRepository.SaveFolderFieldsObj(savedobj);

            //    var fieldList = CCFieldRepository.CCFolderFields.Where(f => f.AccountGUID == accountObj.AccountGUID & f.FolderID == fold.FolderID).ToList();

            //    var toInActiveList = fieldList.Except(activeFields).ToList();

            //    int flag = 0;

            //    foreach (var f1 in fieldList)
            //    {
            //        flag = 0;
            //        foreach (var f2 in activeFields)
            //        {
            //            try
            //            {
            //                if (f2.FieldID == f1.FieldID)
            //                {
            //                    if (f1.isActive != false)
            //                    {
            //                        flag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        flag = 2;
            //                        break;
            //                    }

            //                }
            //            }
            //            catch (Exception ex)
            //            { }

            //        }

            //        if (flag == 0)
            //        {
            //            f1.isActive = false;
            //            CCFieldRepository.SaveFolderFields(f1);
            //        }

            //        else if (flag == 2)
            //        {
            //            f1.isActive = true;
            //            CCFieldRepository.SaveFolderFields(f1);
            //        }
            //    }

            //    // add field into default group            
            //    var defaultGrp = CCGroupRepository.CCGroups.Where(fid => fid.FolderID == fold.FolderID & fid.GroupName == "Default" & fid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();

            //    if (defaultGrp != null)
            //    {
            //        foreach (var field in resp)
            //        {
            //            long grpFieldOrder = 0;
            //            // get group fileds last order
            //            var groupFieldOrder = CCGroupFieldRepository.CCGroupsFields.Where(gid => gid.GroupID == defaultGrp.GroupID & gid.AccountGUID == accountObj.AccountGUID).OrderByDescending(grp => grp.FieldOrder).FirstOrDefault();
            //            if (groupFieldOrder != null) { grpFieldOrder = groupFieldOrder.FieldOrder; }

            //            CCGroupField objgrpfield = new CCGroupField();
            //            objgrpfield.FieldID = field.FieldID;
            //            objgrpfield.FieldOrder = grpFieldOrder + 1;
            //            objgrpfield.FolderID = defaultGrp.FolderID;
            //            objgrpfield.GroupID = defaultGrp.GroupID;
            //            objgrpfield.AccountGUID = accountObj.AccountGUID;
            //            var aa = CCGroupFieldRepository.SaveGroupField(objgrpfield);
            //        }
            //    }
            //}

            

            ////Ajust Folder Fields END





            //// change the plain id
            var planid = planRepository.Plans.FirstOrDefault(pl => pl.PlanLevel == planLevel).ID;

            var currentAccount = accountRepo.Accounts.FirstOrDefault(ac => ac.ID == userObj.AccountID);
            
            //Account account = new Account();
            //account = currentAccount;
            //existingPlanID = account.PlanID;
            //account.PlanID = planid;

            var account = saveFeaturesCommon(plan, quantities);

            account.HasPurchased = true;

            if (currentAccount.TrialEnds <= DateTime.Now.Date & currentAccount.HasPurchased == false & currentAccount.isOverFlow == true)
            {
                account.isOverFlow = false;
            }

            Session["trialData"] = null;

            if (existingPlanID > planid)
            {
                purchaseType = "Downgrade";
            }

            if (accountObj.isPaymentIssue == true)
            {
                account.isPaymentIssue = false;
            }

            account = accountRepo.SaveAccount(account);

            LimitationsViewModel limitationsObj = (LimitationsViewModel)Session["limitations"];
            HelperFunctions HF = new HelperFunctions();
            limitationsObj = HF.updateAccountLimitations(account);
            Session["limitations"] = limitationsObj;

            HF.CheckAcccountStatus(account);

            //Reset Sync Times
            var ConnectionList = CCConnectionRepository.CCSubscriptions.Where(con => con.AccountGUID == account.AccountGUID).ToList();

            foreach (var con in ConnectionList)
            {
                con.ResetAtNextSync = true;
                CCConnectionRepository.SaveSubscription(con);
            }

            var folderDetails = CCFolderRepository.CCFolders.Where(f => f.AccountGUID == account.AccountGUID).ToList();

            foreach (var fold in folderDetails)
            {
                if (fold.isOverFlow == true)
                {
                    if (limitationsObj.folderList.Where(f => f.fold.FolderID == fold.FolderID).FirstOrDefault().itemCount <= limitationsObj.maxItemCountPerFolder)
                    {
                        fold.isOverFlow = false;
                        CCFolderRepository.SaveFolder(fold);
                    }
                }
            }

            Session["account"] = account;

            

            //var featureQuality = featureRepository.Features.Where(pid => pid.PlanLevel == planLevel & pid.Type == "Max Items per Folder").FirstOrDefault();
            //var savedQuality = purchRepository.Purchases.Where(fid => fid.FeatureID == featureQuality.ID && fid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();

            //if (savedQuality != null)
            //{
            //    var quantitySaved = (savedQuality.Quantity) / (featureQuality.Quantity);
            //    limitationsObj.availableCconnectionCount = (int)(quantitySaved * 5) - (int)CCConnectionRepository.CCSubscriptions.Where(C => C.AccountGUID == accountObj.AccountGUID).Count();
            //    limitationsObj.maxItemCountPerFolder = featureQuality.Quantity;
            //    if (featureRepository.Features.Where(pid => pid.PlanLevel == planLevel & pid.Type == "Sync Calendar").FirstOrDefault().Quantity == 0)
            //        limitationsObj.isCalendarSyncAvailable = false;
            //    else
            //        limitationsObj.isCalendarSyncAvailable = true;
            //}

            List<NotificationListViewModel> notificationList = new List<NotificationListViewModel>();
            notificationList = HF.generateNotificationList(account);
            if (notificationList.Count > 0)
                Session["notifications"] = notificationList;
            else
                Session["notifications"] = null;
            
                    
            var currentAccountDetails = accountRepo.Accounts.FirstOrDefault(aid => aid.ID == userObj.AccountID);

            if (currentAccountDetails.StripeCustomerID != null)
            {
                ChargeFromExistingDetails(existingPlanID, quantities, couponID);
            }

            if (account != null)
            {
                if (currentAccountDetails.StripeCustomerID == null)
                {
                    return Json("SucessNew", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Sucess", JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }

        }

        public void ChargeFromExistingDetails(int existingPlanID, int quantities, string couponID)
        {
            // get selected plan
            User userObj = (User)Session["user"];
            var planID = accountRepo.Accounts.FirstOrDefault(aid => aid.ID == userObj.AccountID).PlanID;

            // get selected plan details
            Plan selectedPlanName = new Plan();
            selectedPlanName = planRepository.Plans.FirstOrDefault(pid => pid.ID == planID);

            // get Existing plan name
            var existingPlan = planRepository.Plans.FirstOrDefault(pid => pid.ID == existingPlanID);

            var customerService = new StripeCustomerService();
            customerService.ApiKey = "sk_test_4Xusc3Meo8gniONh6dDRZvlp";

            var currentAccountDetails = accountRepo.Accounts.FirstOrDefault(aid => aid.ID == userObj.AccountID);
            if (currentAccountDetails.StripeCustomerID != null)
            {
                //update subscription 
                var subscribeDetails = customerService.Get(currentAccountDetails.StripeCustomerID);
                var allCurrentSubscriptions = subscribeDetails.StripeSubscriptionList.StripeSubscriptions.Where(sid => sid.CustomerId == currentAccountDetails.StripeCustomerID).ToList();
                var subID = "";
                try
                {
                    subID = subscribeDetails.StripeSubscriptionList.StripeSubscriptions.FirstOrDefault(sid => sid.CustomerId == currentAccountDetails.StripeCustomerID && sid.StripePlan.Name == existingPlan.Name).Id;
                }
                catch(Exception ex)
                {
                    subID = subscribeDetails.StripeSubscriptionList.StripeSubscriptions.FirstOrDefault(sid => sid.CustomerId == currentAccountDetails.StripeCustomerID && sid.StripePlan.Name == "Empty Plan").Id;
                }
                
                var subs = new StripeSubscriptionService();
                subs.ApiKey = "sk_test_4Xusc3Meo8gniONh6dDRZvlp";
                if (couponID != "")
                {
                    var updateRes = subs.Update(currentAccountDetails.StripeCustomerID, subID, new StripeSubscriptionUpdateOptions() { PlanId = selectedPlanName.Name, Quantity = quantities, CouponId = couponID });
                }
                else
                {
                    var updateRes = subs.Update(currentAccountDetails.StripeCustomerID, subID, new StripeSubscriptionUpdateOptions() { PlanId = selectedPlanName.Name, Quantity = quantities});
                }


            }
        }

        public ActionResult GetFeaturesPrice(string plan, int quantitites)
        {
            decimal selectedPackageAmount = 0;
            int planLevel = Int32.Parse(plan);
            var featuresForPlan = featureRepository.Features.Where(pl => pl.PlanLevel == planLevel).ToList();

            //get select package amount
            selectedPackageAmount = (planRepository.Plans.FirstOrDefault(pl => pl.PlanLevel == planLevel).Price) * quantitites;

            return Json((selectedPackageAmount).ToString("0.00"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDiscountedPrice(string plan, int quantitites, string couponID)
        {
            decimal selectedPackageAmount = 0;
            int planLevel = Int32.Parse(plan);
            int Couponid = Int32.Parse(couponID);
            if (Couponid == 111222)
            {
                var featuresForPlan = featureRepository.Features.Where(pl => pl.PlanLevel == planLevel).ToList();

                //get select package amount
                selectedPackageAmount = ((planRepository.Plans.FirstOrDefault(pl => pl.PlanLevel == planLevel).Price) * quantitites) - (((planRepository.Plans.FirstOrDefault(pl => pl.PlanLevel == planLevel).Price) * quantitites) * 10 / 100);
            }
            else if (Couponid == 333444)
            {
                var featuresForPlan = featureRepository.Features.Where(pl => pl.PlanLevel == planLevel).ToList();

                //get select package amount
                selectedPackageAmount = ((planRepository.Plans.FirstOrDefault(pl => pl.PlanLevel == planLevel).Price) * quantitites) - (((planRepository.Plans.FirstOrDefault(pl => pl.PlanLevel == planLevel).Price) * quantitites) * 15 / 100);
            }

            return Json((selectedPackageAmount).ToString("0.00"), JsonRequestBehavior.AllowGet);


        }


        public ActionResult AddNewCard(string stripeToken)
        {
            bool tokenUsed = false;

            User userObj = (User)Session["user"];
            if (userObj.PrimaryUser == false)
                return RedirectToAction("Index", "Admin");  
            var stripeCustomerID = accountRepo.Accounts.FirstOrDefault(aid => aid.ID == userObj.AccountID).StripeCustomerID;

            if (stripeCustomerID == null)
            {
                // create subscription and get subscription id
                var customerService = new StripeCustomerService();
                customerService.ApiKey = "sk_test_4Xusc3Meo8gniONh6dDRZvlp";

                var currentAccountDetails = accountRepo.Accounts.FirstOrDefault(aid => aid.ID == userObj.AccountID);

                var customer = new StripeCustomerCreateOptions();
                customer.PlanId = "Empty Plan";
                customer.Quantity = 1;
                customer.TokenId = stripeToken;
                customer.Email = userObj.Email;

                // Create subscription
                var subscriptionDetails = customerService.Create(customer);
                stripeCustomerID = subscriptionDetails.Id;

                // save StripeCustomerID          
                currentAccountDetails.StripeCustomerID = stripeCustomerID;
                accountRepo.SaveAccount(currentAccountDetails);

                tokenUsed = true;
            }
            else
            { 
                var card = new StripeCardService();
                card.ApiKey = "sk_test_4Xusc3Meo8gniONh6dDRZvlp";

                var cardOpt = new StripeCardCreateOptions();
                cardOpt.TokenId = stripeToken;

                var res = card.Create(stripeCustomerID, cardOpt);
            }


            return RedirectToAction("BillingOptions", "Admin", new { id = 1 });
        }

        public ActionResult DeleteCard(string id)
        {
            User userObj = (User)Session["user"];
            var stripeCustomerID = accountRepo.Accounts.FirstOrDefault(aid => aid.ID == userObj.AccountID).StripeCustomerID;

            var card = new StripeCardService();
            card.ApiKey = "sk_test_4Xusc3Meo8gniONh6dDRZvlp";
            card.Delete(stripeCustomerID, id);

            //return RedirectToAction("Subscription", "Admin", new { id = 1 });
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeDefault(string id)
        {
            User userObj = (User)Session["user"];
            var stripeCustomerID = accountRepo.Accounts.FirstOrDefault(aid => aid.ID == userObj.AccountID).StripeCustomerID;

            var customer = new StripeCustomerService();
            customer.ApiKey = "sk_test_4Xusc3Meo8gniONh6dDRZvlp";

            var customerUpdate = new StripeCustomerUpdateOptions();
            customerUpdate.DefaultCard = id;
            customer.Update(stripeCustomerID, customerUpdate);

            return RedirectToAction("Subscription", "Admin", new { id = 1 });
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
                    RecipientAddress = "brendan@davtonuk.com",
                    Message = "Hello, \n\n" + "Exception message:-\n" + messageExce + "\n\nControllerName:- " + controllerName + "\n\nActionName:- " + actionNmae + "\n\nUserName:- " + userName + "\n\nUserID:- " + userObj.ID + "\n\nAccountID:- " + userObj.AccountID
                };
                notifManager.SendNotification(notifObj);
            }
            else
            {
                var notifObj = new Notification()
                {
                    Subject = " Corporate contacts !",
                    RecipientAddress = "brendan@davtonuk.com",
                    Message = "Hello, \n\n" + "Error on creating account \n\n Exception message:-\n" + messageExce
                };
                notifManager.SendNotification(notifObj);
            }
            return View();
        }

        [HttpPost]
        public ActionResult EditAccountInfor(AccountInfomationViewModel accountinfor)
        {
            User userObj = (User)Session["user"];
            if (userObj != null)
            {
                if (userObj.PrimaryUser == false)
                    return RedirectToAction("Index", "Admin");  
                Account account = new Account();
                account = accountRepo.Accounts.Where(id => id.ID == userObj.AccountID).FirstOrDefault();
                account.TimeZone = accountinfor.TimeZone;
                account.BusinessAddress = accountinfor.BillingAddress;

                var res = accountRepo.UpdateAccountInfo(account);
                if (res) { Session["timeZone"] = accountinfor.TimeZone; }
            }

            return RedirectToAction("AccountOptions", "Admin");
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
                    customer.ApiKey = "sk_test_4Xusc3Meo8gniONh6dDRZvlp";
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
                    cards.ApiKey = "sk_test_4Xusc3Meo8gniONh6dDRZvlp";
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

        public ActionResult BillingOptions(int id)
        {
            TempData["SelectedMenu"] = "Account";
            TempData["SelectedSubMenu"] = "Billing";
            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];

            if (userObj.PrimaryUser == false)
                return RedirectToAction("Index", "Admin");  

            decimal additionalPrice = 0;
            //TempData["SelectedMenu"] = "Subscription";
            List<List<string>> typeListObj = new List<List<string>>();

            PackageViewModel packageViewModel = new PackageViewModel();

            //trial Data
            if (Session["trialData"] != null)
            {
                TrialDataModel trialObj = (TrialDataModel)Session["trialData"];
                packageViewModel.trialObj = trialObj;
            }

            //get all plans
            var plans = planRepository.Plans.ToList();
            // plans.RemoveAll(pname => pname.Name == "Free");
            packageViewModel.Plans = plans;

            // get all types
            //var types = featureRepository.Features.Where(p => p.Price == 0).Select(f => f.Type).Distinct().ToList();
            List<string> types = new List<string>();
            types.Add("Sync Contacts");
            types.Add("Sync Calendar");
            types.Add("Sync Period");
            //types.Add("Contacts Sync");
            //types.Add("Calendar Sync");
            //types.Add("Connections");
            types.Add("Max Items per Folder");
            types.Add("Fields Available");
            types.Add("History Tracking");
            types.Add("Third Party Connections");
            packageViewModel.Types = types;

            // get selected plan            
            var planID = accountRepo.Accounts.FirstOrDefault(aguid => aguid.AccountGUID == accountObj.AccountGUID).PlanID;
            packageViewModel.PlanID = planID;

            // get selected plan details 
            var selectedPlanDetails = plans.FirstOrDefault(pid => pid.ID == planID);
            selectedPlanDetails.Price = selectedPlanDetails.Price;
            packageViewModel.SelectedPlanDetails = selectedPlanDetails;


            // run all plans
            foreach (var pln in plans)
            {

                var allFeatures = featureRepository.Features.Where(pl => pl.PlanLevel == pln.PlanLevel).ToList();

                List<string> typesList = new List<string>();

                foreach (var typ in types)
                {

                    var res = allFeatures.FirstOrDefault(t => t.Type == typ);

                    if (res != null)
                    {
                        typesList.Add(res.Name);
                    }
                    else
                    {
                        typesList.Add("-");
                    }

                }

                typeListObj.Add(typesList);

            }

            packageViewModel.Names = typeListObj;

            // get card details from stripe
            var stripeCustomerID = accountRepo.Accounts.FirstOrDefault(aguid => aguid.AccountGUID == accountObj.AccountGUID).StripeCustomerID;

            if (stripeCustomerID != null)
            {
                // default card details 
                var customer = new StripeCustomerService();
                customer.ApiKey = "sk_test_4Xusc3Meo8gniONh6dDRZvlp";
                var cusRes = customer.Get(stripeCustomerID);
                var defaultCardID = cusRes.StripeDefaultCardId;
                packageViewModel.DefaultCardID = defaultCardID;

                // get saved quantity
                var featureQuality = featureRepository.Features.FirstOrDefault(pid => pid.PlanLevel == selectedPlanDetails.PlanLevel & pid.Type == "Max Items per Folder");
                var savedQuality = purchRepository.Purchases.FirstOrDefault(fid => fid.FeatureID == featureQuality.ID && fid.AccountGUID == accountObj.AccountGUID);

                if (savedQuality != null)
                {
                    var quantitySaved = (savedQuality.Quantity) / (featureQuality.Quantity);
                    packageViewModel.QuantitySaved = quantitySaved;

                }
                else
                {
                    packageViewModel.QuantitySaved = 1;
                }

                var cards = new StripeCardService();
                cards.ApiKey = "sk_test_4Xusc3Meo8gniONh6dDRZvlp";
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
                packageViewModel.CardDetails = allCardDetails;
            }

            else
            {
                packageViewModel.CardDetails = null;
            }


            // End get card details 

            if (id == 2)
            {
                ViewBag.MessagePass = "Your package has successfully changed";
            }
            else
            {
                ViewBag.MessagePass = "";
            }

            return View(packageViewModel);
        }

        public ActionResult checkFeaturesForDowngrade(int newPlanID, int newPlanQty)
        {
            Boolean rejectDowngrade = false;
            string ErrMsgs = "";

            User userObj = (User)Session["user"];
            Account accountObj = (Account)Session["account"];

            var exPlanDetails = planRepository.Plans.Where(p => p.ID == accountObj.PlanID).FirstOrDefault();
            var newPlanDetails = planRepository.Plans.Where(p=>p.PlanLevel == newPlanID).FirstOrDefault();

            //var exFeatureList = featureRepository.Features.Where(f => f.PlanLevel == exPlanDetails.PlanLevel).ToList();
            var newFeatureList = featureRepository.Features.Where(f => f.PlanLevel == newPlanDetails.PlanLevel).ToList();
            
            // get saved quantity
            var featureQtyNew = featureRepository.Features.FirstOrDefault(pid => pid.PlanLevel == newPlanDetails.PlanLevel & pid.Type == "Max Items per Folder");
            var featureQtyOld = featureRepository.Features.FirstOrDefault(pid => pid.PlanLevel == exPlanDetails.PlanLevel & pid.Type == "Max Items per Folder");
            var savedQty = purchRepository.Purchases.FirstOrDefault(fid => fid.FeatureID == featureQtyOld.ID && fid.AccountGUID == accountObj.AccountGUID);
            //var quantitySaved = 0;

            //if (savedQty != null)
            //{
            //    quantitySaved = (savedQty.Quantity) / (featureQtyOld.Quantity);
            //}

            //var newQtyUsed = 5

            //Check No. of Connections with new Plan
            var currConnectionCount = CCConnectionRepository.CCSubscriptions.Where(c => c.AccountGUID == accountObj.AccountGUID).Count();

            if (newPlanQty * 5 < currConnectionCount)
            {
                rejectDowngrade = true;
                var exceessCount = currConnectionCount - (newPlanQty * 5);
                ErrMsgs = ErrMsgs + "<li><p><i class='icon-remove red'></i>&nbsp;Cannot downgrade plan because you have " + exceessCount + " Connections more than the new Plan. Please delelte any exsisiting Connections.</p></li><br/>";
            }

            //Check Calendar Folder
            var calendarFeature = featureRepository.Features.FirstOrDefault(pid => pid.PlanLevel == newPlanDetails.PlanLevel & pid.Type == "Sync Calendar");

            //Check No. of Max Items Per Folder
            var folderList = CCFolderRepository.CCFolders.Where(f => f.AccountGUID == accountObj.AccountGUID).ToList();

            foreach (var fold in folderList)
            {
                var folderItemCount = CCItemRepository.CCContacts.Where(i => i.FolderID == fold.FolderID & i.isDeleted == false).Count();
                if (folderItemCount > featureQtyNew.Quantity)
                {
                    rejectDowngrade = true;
                    ErrMsgs = ErrMsgs + "<li><p><i class='icon-remove red'></i>&nbsp;Cannot downgrade plan because Folder '" + fold.Name + "' exceeds the Max Item count per Folder.</p></li><br/>";
                }
                if (calendarFeature.Quantity == 0 & fold.Type == 2)
                {
                    rejectDowngrade = true;
                    ErrMsgs = ErrMsgs + "<li><p><i class='icon-remove red'></i>&nbsp;Cannot downgrade plan because you have Appointment Folders, which the new Plan will not support. Please make sure to delete the Appointment Folders before downgrade.</p></li><br/>";
                }
            }

            if (rejectDowngrade == true)
            {
                return Json(ErrMsgs, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("success", JsonRequestBehavior.AllowGet);
            }

            
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

        public ActionResult SystemHealthStatus()
        {
            TempData["SelectedMenu"] = "SystemStatus";
            TempData["SelectedSubMenu"] = "";

            HealthMsgsViewModel healthMsgObj = new HealthMsgsViewModel();
            healthMsgObj.HealthMsgList = CCHealthMsgsRepository.CCHealthMsgs.OrderByDescending(HM => HM.ID).ToList();

            return View(healthMsgObj);
        }

        public FileResult GetFile()
    {
        string fileName = "CorporateContactsSetupInstructions.pdf";
     
       string path = AppDomain.CurrentDomain.BaseDirectory + "App_Data/";            
       return File(path + fileName, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
}


    }
}
