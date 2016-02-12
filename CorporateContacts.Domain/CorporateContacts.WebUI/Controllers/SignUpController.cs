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
using CaptchaMvc.HtmlHelpers;
using System.Text.RegularExpressions;
using System.Configuration;
using CorporateContacts.Domain.Concrete;

namespace CorporateContacts.WebUI.Controllers
{
    [HandleError]
    public class SignUpController : Controller
    {
        //
        // GET: /SignUp/
        private IPlanRepo planRepository;
        private IAccountRepo accountRepository;
        private IFeatureRepo featureRepository;
        private IUserRepo userRepository;
        private IPurchasedFeatureRepo ppRepository;
        private IAuthProvider authProvider;
        private ICCFolderRepo CCFolderRepository;
        private ICCTokenRepo CCTokenRepository;
        private INotificationManager notifManager;
        private string rand = "00062008-0000-0000-C000-000000000046";

        public SignUpController(IPlanRepo repo, IAccountRepo accRepo, IFeatureRepo featureRepo, IUserRepo userRepo, IPurchasedFeatureRepo ppRepo, IAuthProvider auth, ICCFolderRepo folder, ICCTokenRepo token, INotificationManager notifMgr)
        {
            planRepository = repo;
            accountRepository = accRepo;
            featureRepository = featureRepo;
            userRepository = userRepo;
            ppRepository = ppRepo;
            authProvider = auth;
            CCFolderRepository = folder;
            CCTokenRepository = token;
            notifManager = notifMgr;
            //SetConnectionString();
        }

        // Set Connection string 
        private EFDbContextAccounts context = new EFDbContextAccounts();

        public ViewResult ProductList()
        {
            return View(planRepository.Plans);
        }

        [HttpGet]
        public ActionResult SaveAccount()
        {
            SignUpViewModel signupViewModel = new SignUpViewModel();

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
            signupViewModel.TimeZoneList = tzList;
            return View(signupViewModel);
        }

        [HttpPost]
        public ActionResult SaveAccount(SignUpViewModel sgnv)
        {

            if (ModelState.IsValid)
            {
                // check email already exist
                var isEmailExist = userRepository.Users.Where(email => email.Email == sgnv.User.Email).ToList();
                var isAccNameExist = accountRepository.Accounts.Where(acc => acc.AccountName == sgnv.Account.AccountName).ToList();

                if (isEmailExist.Count() == 0 && isAccNameExist.Count() ==0)
                {
                    //save account details
                    sgnv.Account.AccountGUID = System.Guid.NewGuid().ToString();
                    var account = accountRepository.SaveAccount(sgnv.Account);
                    account.TablePrefix = account.ID.ToString("X");
                    account.PlanID = 2;
                    account.TimeZone = sgnv.Account.TimeZone;
                    account.Telephone = sgnv.Account.Telephone;
                    account.CreatedDate = DateTime.Now;
                    account.LastModifiedDate = DateTime.Now;
                    account.HasPurchased = false;
                    account.isOverFlow = false;
                    account.isPaymentIssue = false;
                    account.SyncPeriod = (short)featureRepository.Features.Where(pl => pl.PlanLevel == 20 && pl.Type == "Sync Period").First().Quantity;
                    account = accountRepository.SaveAccount(sgnv.Account);
                    Session["accountid"] = account.ID;

                    // save user details 
                    sgnv.User.UserType = "Admin";
                    sgnv.User.PrimaryUser = true;
                    sgnv.User.AccountID = (long)Session["accountid"];
                    sgnv.User.Password = Encryption.EncryptStringAES(sgnv.User.Password, rand);
                    sgnv.User.isPasswordChange = true;
                    sgnv.User.ConfirmPassword = sgnv.User.Password;
                    sgnv.User.CreatedDate = DateTime.Now;
                    sgnv.User.LastModifiedDate = DateTime.Now;
                    var user = userRepository.SaveUser(sgnv.User);


                    // save package
                    var featuresForDefaultPlan = featureRepository.Features.Where(pl => pl.PlanLevel == 20).ToList();

                    foreach (var feat in featuresForDefaultPlan)
                    {
                        PurchasedFeatures objFeature = new PurchasedFeatures();
                        objFeature.AccountGUID = account.AccountGUID;
                        objFeature.FeatureID = feat.ID;
                        objFeature.ExpiryDate = DateTime.UtcNow;
                        objFeature.Enabled = true;
                        objFeature.Quantity = feat.Quantity;
                        ppRepository.SavePurchase(objFeature);
                    }

                    // send Email Notification
                    user.GUID = Guid.NewGuid().ToString();
                    var notifObj = new Notification()
                    {
                        Subject = "Welcome to Corporate contacts !",
                        RecipientAddress = user.Email,
                        //RecipientAddress = "brendan@davtonuk.com",
                        // Message = "Hello, \n\n" + user.FullName + " \nAccount successfully created.\nPlease click the link below to activate your account.\n\n" + Request.Url.GetLeftPart(UriPartial.Authority) + "/VerifyUserAccount/VerifyEmailAddress?GUID=" + user.GUID + "\n\nCorporate Contacts Team"
                        //Message = "Dear " + user.FullName + ",\n"
                        //+ "Thank you for registering with Corporate Contacts. You have created a new account:\n\n"
                        //+ "User Name: " + user.FullName + "\nEmail: " + user.Email +
                        //"\n\nYou still need to click the link below to verify your email address. Please follow the link below:  \n" + Request.Url.GetLeftPart(UriPartial.Authority) + "/VerifyUserAccount/VerifyEmailAddress?GUID=" + user.GUID +
                        //"\n\nIf you need help, you can find it here: http://support.corporate-contacts.com \n\nregards \nThe Corporate Contacts team \n\n(If you have received this email in error, or have any other concerns, please email support@corporate-contacts.com to let us know.)"

                        Message = "<div><center> <table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> </tr><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td align='center' valign='top' width='600' style='padding-left:0;padding-right:0;padding-top:0;padding-bottom:0;border-top:1px solid #cccccc;border-right:1px solid #cccccc;border-left:1px solid #cccccc;background-color:#ffffff'><img src='http://corporate-contacts.com/wp-content/uploads/2014/03/CorporateContactslogo3a2.png' style='max-width: 450px;margin-top: 20px;' alt='Dyn'></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc;background-color:#ffffff'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color:#ffffff'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='top' style='padding-top:20px;padding-right:30px;padding-left:30px'><table align='left' border='0' cellpadding='0' cellspacing='0'> <tbody><tr> <td valign='top' style='color:#444444;font-family:Helvetica;font-size:16px;line-height:125%;text-align:left;padding-bottom:20px'><p>Dear " + user.FullName + "</p><p>Thank you for registering with Corporate Contacts. You have created a new account</p><p><strong>Login Email :</strong> " + user.Email + "</p><p>You still need to <a href='" + Request.Url.GetLeftPart(UriPartial.Authority) + "/VerifyUserAccount/VerifyEmailAddress?GUID=" + user.GUID + "'>Click Here</a> to Activate your account or Please follow the link below:</p><p><a style='text-decoration:underline;color:#0066cc' href='" + Request.Url.GetLeftPart(UriPartial.Authority) + Url.RouteUrl(new { controller = "VerifyUserAccount", action = "VerifyEmailAddress" }) + "?GUID=" + user.GUID + "' target='_blank'>" + Request.Url.GetLeftPart(UriPartial.Authority) + Url.RouteUrl(new { controller = "VerifyUserAccount", action = "VerifyEmailAddress" }) + "?GUID=" + user.GUID + "</a></p><p>If you did not sign up for this account, this will be the onlycommunication you will receive. All non-confirmed accounts areautomatically deleted after 48 hours and no addresses are kept on file. Weapologize for any inconvenience this correspondence may have caused and weassure you that it was only sent at the request of someone visiting oursite requesting an account.</p><p>If you need help, you can find it here: <a href='http://support@corporate-contacts.com'>support@corporate-contacts.com</a></p><p>Regards,<br><a style='text-decoration:underline;color:#0066cc' href='http://corporate-contacts.com/' target='_blank'>The Corporate Contacts team</a></p></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color: #209FD1;'> <tbody><tr> <td align='center' valign='bottom'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td valign='bottom' width='600' style='padding-top:0px;padding-right:20px;padding-left:20px;padding-bottom:10px'><table align='left' border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Corporate Contacts Ltd.<br>Beverley, UK, HU17 0LD<br>Manchester, NH 03101</td><td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Phone: +44 1482 869700<br>E-Mail: hello@corporate-contacts.com</td></tr></tbody></table> </td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></center></div>"
                    };
                    notifManager.SendNotification(notifObj);

                    // save state 
                    user.Status = "Invite Sent";
                    user = userRepository.SaveUser(user);
                    return RedirectToAction("Success");
                }

                else
                {
                    ModelState.Clear();
                    if (isEmailExist.Count() != 0)
                        ViewBag.EmailValidation = "Please enter a different email, this email already exists";

                    if (isAccNameExist.Count() != 0)
                        ViewBag.AccountNameValidation = "Please enter a different Account Name, this is already in use";    

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
                    sgnv.TimeZoneList = tzList;

                    sgnv.User.Email = "";
                    return View(sgnv);
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult ResendVerification(long uid)
        {
            User userObj = (User)Session["user"];
            if (userObj != null)
            {
                return View(userObj);
            }
            return View();
        }

        public ActionResult ResendVerificationLink(long uid)
        {
            var user = userRepository.Users.Where(id => id.ID == uid).FirstOrDefault();

            if (user != null)
            {
                // send Email Notification
                user.GUID = Guid.NewGuid().ToString();
                var notifObj = new Notification()
                {
                    Subject = "Welcome to Corporate contacts !",
                    RecipientAddress = user.Email,
                    //RecipientAddress = "brendan@davtonuk.com",
                    //Message = "Hello, \n\n" + user.FullName + " \nAccount successfully created.\nPlease click the link below to activate your account.\n\n" + Request.Url.GetLeftPart(UriPartial.Authority) + "/VerifyUserAccount/VerifyEmailAddress?GUID=" + user.GUID + "\n\nCorporate Contacts Team"

                    //Message = "Dear " + user.FullName + ",\n"
                    //        + "Thank you for registering with Corporate Contacts. You have created a new account:\n\n"
                    //        + "User Name: " + user.FullName + "\nEmail: " + user.Email +
                    //        "\n\nYou still need to click the link below to verify your email address. Please follow the link below:  \n" + Request.Url.GetLeftPart(UriPartial.Authority) + "/VerifyUserAccount/VerifyEmailAddress?GUID=" + user.GUID +
                    //        "\n\nIf you need help, you can find it here: http://support.corporate-contacts.com \n\nregards \nThe Corporate Contacts team \n\n(If you have received this email in error, or have any other concerns, please email support@corporate-contacts.com to let us know.)"

                    Message = "<div><center> <table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> </tr><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td align='center' valign='top' width='600' style='padding-left:0;padding-right:0;padding-top:0;padding-bottom:0;border-top:1px solid #cccccc;border-right:1px solid #cccccc;border-left:1px solid #cccccc;background-color:#ffffff'><img src='http://corporate-contacts.com/wp-content/uploads/2014/03/CorporateContactslogo3a2.png' style='max-width: 450px;margin-top: 20px;' alt='Dyn'></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc;background-color:#ffffff'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color:#ffffff'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='top' style='padding-top:20px;padding-right:30px;padding-left:30px'><table align='left' border='0' cellpadding='0' cellspacing='0'> <tbody><tr> <td valign='top' style='color:#444444;font-family:Helvetica;font-size:16px;line-height:125%;text-align:left;padding-bottom:20px'><p>Dear " + user.FullName + "</p><p>Thank you for registering with Corporate Contacts. You have created a new account</p><p><strong>Login Email :</strong> " + user.Email + "</p><p>You still need to <a href='" + Request.Url.GetLeftPart(UriPartial.Authority) + "/VerifyUserAccount/VerifyEmailAddress?GUID=" + user.GUID + "'>Click Here</a> to Activate your account or Please follow the link below:</p><p><a style='text-decoration:underline;color:#0066cc' href='" + Request.Url.GetLeftPart(UriPartial.Authority) + "/VerifyUserAccount/VerifyEmailAddress?GUID=" + user.GUID + "' target='_blank'>" + Request.Url.GetLeftPart(UriPartial.Authority) + "/VerifyUserAccount/VerifyEmailAddress?GUID=" + user.GUID + "</a></p><p>If you did not sign up for this account, this will be the onlycommunication you will receive. All non-confirmed accounts areautomatically deleted after 48 hours and no addresses are kept on file. Weapologize for any inconvenience this correspondence may have caused and weassure you that it was only sent at the request of someone visiting oursite requesting an account.</p><p>If you need help, you can find it here: <a href='http://support.corporate-contacts.com'>support.corporate-contacts.com</a></p><p>Regards,<br><a style='text-decoration:underline;color:#0066cc' href='http://corporate-contacts.com/' target='_blank'>The Corporate Contacts team</a></p></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color: #209FD1;'> <tbody><tr> <td align='center' valign='bottom'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td valign='bottom' width='600' style='padding-top:0px;padding-right:20px;padding-left:20px;padding-bottom:10px'><table align='left' border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Corporate Contacts Ltd.<br>Beverley, UK, HU17 0LD<br>Manchester, NH 03101</td><td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Phone: +44 1482 869700<br>E-Mail: hello@corporate-contacts.com</td></tr></tbody></table> </td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></center></div>"
                };
                notifManager.SendNotification(notifObj);

                // save state 
                user.Status = "Invite Sent";
                user = userRepository.SaveUser(user);

                ViewBag.VerifiedLinkSucess = "We have sent you an email, Please check your email and active your account";
                ViewBag.VerifiedLinkFail = "";
            }
            else
            {
                ViewBag.VerifiedLinkSucess = "";
                ViewBag.VerifiedLinkFail = "We couldn't send you a verification link, please contact support@corporate-contacts.com";
            }

            return View();
        }

        public ActionResult NewUser(string guid = "")
        {
            var userObj = userRepository.GetUserByGUID(guid);
            if (userObj != null)
            {
                ViewBag.GUID = guid;
                return View();
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult NewUser(PasswordViewModel pvObj)
        {

            if (ModelState.IsValid)
            {
                var userObj = userRepository.GetUserByGUID(pvObj.GUID);
                userObj.Password = pvObj.Password;
                userObj.ConfirmPassword = pvObj.ConfirmPassword;
                userObj.Status = "Active";
                var user = userRepository.SaveUser(userObj);

                //create a session for the user.
                if (authProvider.Authenticate(user.Email, user.Password, userRepository))
                {
                    Session["user"] = userRepository.GetUserByEmailAddress(user.Email);
                }
                if (userObj.UserType == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "User");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please check your password");
                return View();
            }
        }



    }
}
