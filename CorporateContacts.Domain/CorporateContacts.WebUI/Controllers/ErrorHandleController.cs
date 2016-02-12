using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CorporateContacts.Domain.Concrete;
using CorporateContacts.Domain.Entities;
using CorporateContacts.Domain.Abstract;

namespace CorporateContacts.WebUI.Controllers
{
    public class ErrorHandleController : Controller
    {
        INotificationManager notifManager;
        ICCErrorLogRepo CCErrorLogRepository;

        public ErrorHandleController(INotificationManager notifMgr, ICCErrorLogRepo errorlog)
        {          
            notifManager = notifMgr;
            CCErrorLogRepository = errorlog;
           
        }
        //
        // GET: /ErrorHandle/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendErrorNotification(string actionNmae, string controllerName, string messageExce, string errorMessage, string errorSource)
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
                    Message = "Hello, \n\n" + "Error on creating or login account \n\n Exception message:-\n" + messageExce
                };
                notifManager.SendNotification(notifObj);
            }
            SaveErrorLogfile(actionNmae, controllerName, messageExce, errorMessage, errorSource);
            return View();
        }

        void SaveErrorLogfile(string actionNmae, string controllerName, string stackTrace, string errorMessage, string errorSource)
        {
            CCErrorLog objerrorlog = new CCErrorLog();
            objerrorlog.DateTime = DateTime.UtcNow;
            objerrorlog.Controller = controllerName;
            objerrorlog.Action = actionNmae;
            objerrorlog.ErrorStackTrace = stackTrace;
            objerrorlog.Source = "Web";
            objerrorlog.ErrorMsgUF = errorMessage;
            objerrorlog.ErrorMsg = errorMessage;
            objerrorlog.ErrorType = "Error";
            objerrorlog.ConnectionID = 0;

             User userObj = (User)Session["user"];
             Account accountObj = (Account)Session["account"]; 
             if (userObj != null)
             {
                 objerrorlog.AccountID = userObj.AccountID;
                 objerrorlog.UserEmail = userObj.Email;
             }
             if (accountObj != null)
             {                
                 objerrorlog.AccountGUID = accountObj.AccountGUID;
             }
            var res = CCErrorLogRepository.SaveErrorLog(objerrorlog);
        
        }
    }
}