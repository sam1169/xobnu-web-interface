using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;
using Xobnu.WebUI.Infrastructure;
using Xobnu.WebUI.Models;
using Xobnu.WebUI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Xobnu.WebUI.Controllers
{
    [HandleError]
    public class VerifyUserAccountController : Controller
    {
        IUserRepo userRepository;
        private IAuthProvider authProvider;
        IAccountRepo accRepository;
        ICCFolderRepo CCFolderRepository;

        private string rand = "00062008-0000-0000-C000-000000000046";

        public VerifyUserAccountController(IUserRepo urep, IAuthProvider auth, IAccountRepo account, ICCFolderRepo folderRep)
        {
            userRepository = urep;
            authProvider = auth;
            accRepository = account;
            CCFolderRepository = folderRep;
        }
        //
        // GET: /VerifyUserAccount/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult VerifyEmailAddress(string GUID)
        {
            var user = userRepository.Users.Where(uid => uid.GUID == GUID).FirstOrDefault();
            if (user != null)
            {
                bool resUser = userRepository.ActiveEmialVerification(user);
                //Session["ChangePassword"] = "Yes";
                if (resUser) 
                {
                    //create a session for the user.
                    var pass = Encryption.DecryptStringAES(user.Password, rand);
                    if (authProvider.Authenticate(user.Email, pass, userRepository))
                    {
                        Session["user"] = userRepository.GetUserByEmailAddress(user.Email);
                        //Session["folderss"] = null;
                        
                        User userObj = (User)Session["user"];
                        var accountObj = accRepository.Accounts.FirstOrDefault(x => x.ID == userObj.AccountID);
                        Session["account"] = accountObj;
                        var folders = CCFolderRepository.CCFolders.Where(guid => guid.AccountGUID == accountObj.AccountGUID).ToList();
                        Session["folderss"] = folders;

                        List<NotificationListViewModel> notificationList = new List<NotificationListViewModel>();
                        HelperFunctions HF = new HelperFunctions();
                        notificationList = HF.generateNotificationList(accountObj);
                        if (notificationList.Count > 0)
                            Session["notifications"] = notificationList;
                        else
                            Session["notifications"] = null;
                    }

                    ViewBag.VerifiedMessageSucess = "You have successfully activated your user account."; ViewBag.VerifiedMessageFail = ""; 
                }
                else { ViewBag.VerifiedMessageFail = "Failed to activate your account."; ViewBag.VerifiedMessageSucess = ""; }
            }
            else
            {
                ViewBag.VerifiedMessageFail = "Sorry, We couldn't find your account.";
                ViewBag.VerifiedMessageSucess = "";
            }
            return View();
        }
    }
}