using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xobnu.Domain.Abstract;
using Xobnu.WebUI.Infrastructure;
using Xobnu.WebUI.Models;
using Xobnu.WebUI.Util;
using Xobnu.Domain.Entities;
using System.Data.Entity;
using Xobnu.Domain.Concrete;
using System.Configuration;

namespace Xobnu.WebUI.Controllers
{
    [HandleError]
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        IAuthProvider authProvider;
        IUserRepo userRepository;
        ICCFolderRepo CCFolderRepository;
        IAccountRepo accRepository;
        ICCTokenRepo CCTokenRepository;
        INotificationManager notifManager;
        ICCItemRepo items;
        ICCErrorLogRepo CCErrorLogRepository;
        private string rand = "00062008-0000-0000-C000-000000000046";

        public LoginController(IAuthProvider auth, IUserRepo userRepo, ICCFolderRepo folder, IAccountRepo account, ICCTokenRepo token, INotificationManager notifMgr, ICCItemRepo item, ICCErrorLogRepo errorlogs)
        {
            authProvider = auth;
            userRepository = userRepo;
            CCFolderRepository = folder;
            accRepository = account;
            CCTokenRepository = token;
            notifManager = notifMgr;
            items = item;
            CCErrorLogRepository = errorlogs;
        }

        public ActionResult Index(string msg = "")
        {
            if (!string.IsNullOrEmpty(msg)) ViewBag.Message = msg;

            var assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ViewBag.version = assemblyVersion;
            Session["version"] = assemblyVersion;

            return View();
        }

        public ActionResult Login(string msg = "")
        {
            if (!string.IsNullOrEmpty(msg)) ViewBag.Message = msg;

            var assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ViewBag.version = assemblyVersion;
            Session["version"] = assemblyVersion;

            return View();
        }

        [HttpPost]
        public ActionResult Login(LogOnViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (authProvider.Authenticate(model.EmailAddress, model.Password, this.userRepository))
                {
                    var userObj = userRepository.GetUserByEmailAddress(model.EmailAddress);
                    Session["user"] = userObj;
                    var accountObj = accRepository.Accounts.FirstOrDefault(x => x.ID == userObj.AccountID);
                    Session["account"] = accountObj;
                    var timeZone = accRepository.Accounts.Where(aid => aid.ID == userObj.AccountID).FirstOrDefault().TimeZone;
                    Session["timeZone"] = timeZone;
                    Session["SysAdminDetails"] = null;

                    List<NotificationListViewModel> notificationList = new List<NotificationListViewModel>();

                    CronJobController CJC = new CronJobController();
                    Session["account"] = CJC.checkAccountTrialExpiryForAccount(accountObj);



                    //save details into error table
                    //SaveLogonDetails(model.EmailAddress);

                    if ( userObj.UserType == "Admin" || userObj.UserType == "Standard")
                    {      
                        if (userObj.EmailVerified == true)
                        {
                            //get the folders  
                            var folders = CCFolderRepository.CCFolders.Where(guid => guid.AccountGUID == accountObj.AccountGUID).ToList();
                            Session["folderss"] = folders;

                            HelperFunctions HF = new HelperFunctions();
                            notificationList = HF.generateNotificationList(accountObj);

                            HF.CheckAcccountStatus(accountObj);

                            if(notificationList.Count>0)
                                Session["notifications"] = notificationList;
                            else
                                Session["notifications"] = null;

                            return Redirect(returnUrl ?? Url.Action("Index", "Admin"));
                        }
                        else
                        {
                            return Redirect(returnUrl ?? Url.Action("ResendVerification", "SignUp", new { uid = userObj.ID }));
                        }
                    }
                    else if (userObj.UserType == "SystemAdmin")
                    {
                        HelperFunctions HF = new HelperFunctions();
                        notificationList = HF.generateNotificationList(accountObj);
                        if (notificationList.Count > 0)
                            Session["notifications"] = notificationList;

                        Session["SysAdminDetails"] = userRepository.GetUserByEmailAddress(model.EmailAddress);

                        return Redirect(returnUrl ?? Url.Action("Index", "CorporateContactsAdmin"));
                    }
                    else
                    {
                        return Redirect(returnUrl ?? Url.Action("SetupSync", "User"));
                    }
                }
                else
                {
                    //save details into error table
                    //SaveLogonDetails(model.EmailAddress);
                    ModelState.AddModelError("", "Incorrect username or password");
                    return View();

                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult SessionExpired()
        {
            Session.Abandon();
            authProvider.SignOut();
            return Redirect("~/Login?msg=Your session expired. Please logon");
        }

        [HttpPost]
        public ActionResult Index(LogOnViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {            
                if (authProvider.Authenticate(model.EmailAddress, model.Password, this.userRepository))
                {
                    var userObj = userRepository.GetUserByEmailAddress(model.EmailAddress);
                    Session["user"] = userObj;
                    var accountObj = accRepository.Accounts.FirstOrDefault(x => x.ID == userObj.AccountID);
                    Session["account"] = accountObj;
                    var timeZone = accRepository.Accounts.Where(aid => aid.ID == userObj.AccountID).FirstOrDefault().TimeZone;
                    Session["timeZone"] = timeZone;

                    //save details into error table
                    SaveLogonDetails(model.EmailAddress);

                    if (userObj.UserType == "Admin" || userObj.UserType == "Standard")
                    {
                        if (userObj.EmailVerified == true)
                        {
                            //get the folders  
                            var folders = CCFolderRepository.CCFolders.Where(guid => guid.AccountGUID == accountObj.AccountGUID).ToList();
                            Session["folderss"] = folders;

                            return Redirect(returnUrl ?? Url.Action("Index", "Admin"));
                        }
                        else
                        {
                            return Redirect(returnUrl ?? Url.Action("ResendVerification", "SignUp", new { uid = userObj.ID }));
                        }
                    }
                    else if (userObj.UserType == "SystemAdmin")
                    {
                        return Redirect(returnUrl ?? Url.Action("Index", "CorporateContactsAdmin"));
                    }
                    else
                    {
                        return Redirect(returnUrl ?? Url.Action("SetupSync", "User"));
                    }
                }
                else
                {
                    //save details into error table
                    SaveLogonDetails(model.EmailAddress);
                    ModelState.AddModelError("", "Incorrect username or password");
                    return View();

                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult ForgotPassword()
        {
            ForgotPasswordModel forgotpassword = new ForgotPasswordModel();

            return View(forgotpassword);
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel forgotpassword)
        {
            var getPassword = "";

            var res = userRepository.Users.Where(umail => umail.Email == forgotpassword.EmailAddress).ToList();

            if (res.Count() > 0)
            {
                getPassword = res.FirstOrDefault(umail => umail.Email == forgotpassword.EmailAddress).Password;
            }

            if (getPassword != "")
            {
                var accoundID = res.FirstOrDefault(umail => umail.Email == forgotpassword.EmailAddress).AccountID;

                string url = HttpContext.Request.Url.AbsoluteUri;
                url = url.Replace("Forgot", "Reset");

                Guid guid = Guid.NewGuid();

                var isExistToken = CCTokenRepository.CCTokens.Where(t => t.AccountID == accoundID);

                if (isExistToken.Count() > 0)
                {
                    var tokenID = isExistToken.FirstOrDefault().ID;
                    CCTokenRepository.DeleteToken(tokenID);
                }

                CCToken objToken = new CCToken();
                objToken.AccountID = accoundID;
                objToken.Token = guid.ToString();
                CCTokenRepository.SaveToken(objToken);

                string body = "Please click on the following link to reset your password " + Environment.NewLine + url +
                                       "?te76emjj55hz=" + guid.ToString();
                var userObj = res.FirstOrDefault(umail => umail.Email == forgotpassword.EmailAddress);

                //new user                
                var notifObj = new Notification()
                {
                    Subject = "Corporate Contacts – Reset password",
                    RecipientAddress = forgotpassword.EmailAddress,
                    Message = "<div><center> <table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> </tr><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td align='center' valign='top' width='600' style='padding-left:0;padding-right:0;padding-top:0;padding-bottom:0;border-top:1px solid #cccccc;border-right:1px solid #cccccc;border-left:1px solid #cccccc;background-color:#ffffff'><img src='http://corporate-contacts.com/wp-content/uploads/2014/03/CorporateContactslogo3a2.png' style='max-width: 450px;margin-top: 20px;' alt='Dyn'></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc;background-color:#ffffff'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color:#ffffff'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='top' style='padding-top:20px;padding-right:30px;padding-left:30px'><table align='left' border='0' cellpadding='0' cellspacing='0'> <tbody><tr> <td valign='top' style='color:#444444;font-family:Helvetica;font-size:16px;line-height:125%;text-align:left;padding-bottom:20px'><p>Dear " + userObj.FullName + "</p><p>Corporate Contacts received a request to reset your password for your account.</p><p>Click on the link below (or copy and paste it) to reset your password.</p><p><a href='" + Request.Url.GetLeftPart(UriPartial.Authority) + Url.RouteUrl(new { controller = "Login", action = "ResetPassword" }) + "?te76emjj55hz=" + guid.ToString() + "'>" + Request.Url.GetLeftPart(UriPartial.Authority) +Url.RouteUrl(new { controller = "Login", action = "ResetPassword" }) + "?te76emjj55hz=" + guid.ToString() + "</a></p><p>If you did not make this request please contact us at www.support.corporate-contacts.com so we can look in to this matter for you.  </p><p>Regards,<br><a style='text-decoration:underline;color:#0066cc' href='http://corporate-contacts.com/' target='_blank'>The Corporate Contacts team</a></p><p><a href='http://www.support.corporate-contacts.com'>www.support.corporate-contacts.com</a></p></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color: #209FD1;'> <tbody><tr> <td align='center' valign='bottom'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td valign='bottom' width='600' style='padding-top:0px;padding-right:20px;padding-left:20px;padding-bottom:10px'><table align='left' border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Corporate Contacts Ltd.<br>Beverley, UK, HU17 0LD<br>Manchester, NH 03101</td><td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Phone: +44 1482 869700<br>E-Mail: hello@corporate-contacts.com</td></tr></tbody></table> </td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></center></div>"
                };
                notifManager.SendNotification(notifObj);

                ViewBag.MessageFail = "";
                ViewBag.MessagePass = "The password reset link has sent to your email. Please check your inbox to continue the password change process"; //FineTune:use green Color in Web page


            }
            else
            {
                ViewBag.MessagePass = "";
                ViewBag.MessageFail = "Invalid email address."; //ToDo:FineTune:Make this Client side
            }


            return View(forgotpassword);
        }

        public ActionResult ResetPassword(string te76emjj55hz) //'te76emjj55hz' stands for token
        {
            string token = te76emjj55hz;
            try
            {
                var isTokenAvailable = CCTokenRepository.CCTokens.Where(t => t.Token == token.Trim()).FirstOrDefault();


                if (isTokenAvailable != null)
                {

                    ForgotPasswordModel forgotPasswordModel = new ForgotPasswordModel();
                    forgotPasswordModel.AccountID = isTokenAvailable.AccountID;


                    return View(forgotPasswordModel);
                }
                else
                {
                    return RedirectToAction("Index", "Admin");//ToDo:FineTune:
                }
            }
            catch (Exception ex)
            {
                //  DebugMessage("LoginController : ActionResult ResetPassword() [Get] : " + ex.ToString(), false);
                return View(); //ASK: is this correct??
            }
        }

        [HttpPost]
        public ActionResult ResetPassword(ForgotPasswordModel viewModel)
        {
            if (ModelState.IsValid)
            {
                User user = new User();
                user = userRepository.Users.FirstOrDefault(aid => aid.AccountID == viewModel.AccountID);
                user.Password = Encryption.EncryptStringAES(viewModel.Password, rand);
                userRepository.SaveUser(user);
                //Encryption.EncryptStringAES(sgnv.User.Password, rand);
            }
            ViewBag.MessagePass = "You have successfully reset your password";
            return View();
        }

        public void SaveLogonDetails(string email)
        {
            Account accountObj = (Account)Session["account"];

            CCErrorLog objerrorlog = new CCErrorLog();
            objerrorlog.ErrorType = "Logon";
            objerrorlog.DateTime = DateTime.UtcNow;
            objerrorlog.Source = "Web";
            objerrorlog.Action = "Index";
            objerrorlog.Controller = "Login";
            objerrorlog.UserEmail = email;

            if (accountObj != null)
            {
                objerrorlog.AccountGUID = accountObj.AccountGUID;
                objerrorlog.AccountID = accountObj.ID;
            }

            var res = CCErrorLogRepository.SaveErrorLog(objerrorlog);

        }

    }
}
