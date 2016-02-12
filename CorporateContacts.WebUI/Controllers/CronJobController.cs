using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Xobnu.WebUI.Util;
using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;
using Xobnu.WebUI.Infrastructure;
using Xobnu.WebUI.Models;
using Xobnu.Domain.Concrete;

namespace Xobnu.WebUI.Controllers
{
    public class CronJobController : ApiController
    {
        IAccountRepo CCaccountRepo;
        INotificationManager notifManager;
        ICCItemRepo CCItemRepository;
        //IPlanRepo CCplanRepository = new EA;
        IUserRepo CCUserRepository;
        ICCFolderRepo CCFolderRepository;
        //IPurchasedFeatureRepo CCpurchRepository;

        public CronJobController()
        {
            CCUserRepository = new EFUserRepo();
            CCItemRepository = new EFCCItemRepo();
            CCFolderRepository = new EFCCFolderRepo();
            //CCplanRepository = pRepo;
            //CCaccountRepo = accRepo;
            //CCpurchRepository = purRepo;
            this.CCaccountRepo = new EFAccountRepo();
            this.notifManager = new EmailNotificationManager();
        }

        public CronJobController(IAccountRepo accRepo ,INotificationManager notifMgr, IUserRepo urep,ICCItemRepo item,ICCFolderRepo fold)
        {
            CCUserRepository = urep;
            //CCplanRepository = pRepo;
            CCaccountRepo = accRepo;
            notifManager = notifMgr;
            CCItemRepository = item;
            CCFolderRepository = fold;
            //CCpurchRepository = purRepo;
            //this.CCaccountRepo = new 
        }

        [HttpGet]
        public xmlClassOutput checkAccountTrialExpiry()
        {
            List<Account> accountList = new List<Account>();
            accountList = CCaccountRepo.Accounts.Where(a=>a.isOverFlow == false).ToList();

            xmlClassOutput resultsObj = new xmlClassOutput();
            resultsObj.title = "List of Expired Accounts";
            resultsObj.accountNameList = new List<string>();

            foreach(var acc in accountList)
            {
                try
                {
                    if ((acc.TrialEnds == DateTime.Now.Date) & (acc.HasPurchased == false))
                    {
                        var user = CCUserRepository.Users.FirstOrDefault(u => u.AccountID == acc.ID && u.UserType != "SystemAdmin");
                        if (user != null)
                        {
                            bool executeStatus = CCaccountRepo.setAccountStatus(true, acc.ID);
                            //expiry email for the customer
                            var notifObj = new Notification()
                            {
                                Subject = "Corporate Contacts Trial Expiry",
                                RecipientAddress = user.Email,
                                Message = "<div><center> <table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> </tr><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td align='center' valign='top' width='600' style='padding-left:0;padding-right:0;padding-top:0;padding-bottom:0;border-top:1px solid #cccccc;border-right:1px solid #cccccc;border-left:1px solid #cccccc;background-color:#ffffff'><img src='http://corporate-contacts.com/wp-content/uploads/2014/03/CorporateContactslogo3a2.png' style='max-width: 450px;margin-top: 20px;' alt='Dyn'></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc;background-color:#ffffff'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color:#ffffff'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='top' style='padding-top:20px;padding-right:30px;padding-left:30px'><table align='left' border='0' cellpadding='0' cellspacing='0'> <tbody><tr> <td valign='top' style='color:#444444;font-family:Helvetica;font-size:16px;line-height:125%;text-align:left;padding-bottom:20px'><p>Dear " + user.FullName + "</p><p>Thank you for trying Corporate Contacts. </p><p>Your 15 day trial period has now expired. We hope you tried all of the features available and have decided on the package that's right for you.  Click <a href='https://secure.corporate-contacts.com/' target='_blank'>here</a> to purchase the account you require and continue using Corporate Contacts.</p><p>Don't worry! If you decide this package is not right for you and your business you can easily select and alternative by visiting the Billings Option page and upgrade or downgrade your chosen package.</p><p>Regards,<br><a style='text-decoration:underline;color:#0066cc' href='http://corporate-contacts.com/' target='_blank'>The Corporate Contacts team</a></p><p><a href='http://www.support.corporate-contacts.com'>www.support.corporate-contacts.com</a></p></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color: #209FD1;'> <tbody><tr> <td align='center' valign='bottom'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td valign='bottom' width='600' style='padding-top:0px;padding-right:20px;padding-left:20px;padding-bottom:10px'><table align='left' border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Corporate Contacts Ltd.<br>Beverley, UK.</td><td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Phone: +44 1482 869700<br>E-Mail: hello@corporate-contacts.com</td></tr></tbody></table> </td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></center></div>"
                            };
                            notifManager.SendNotification(notifObj);

                            //expiry email for admin of CC
                            var notifObjadmin = new Notification()
                            {
                                Subject = "Corporate Contacts Trial Expiry",
                                RecipientAddress = "admin@corporate-contacts.com",
                                Message = "<div><center> <table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> </tr><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td align='center' valign='top' width='600' style='padding-left:0;padding-right:0;padding-top:0;padding-bottom:0;border-top:1px solid #cccccc;border-right:1px solid #cccccc;border-left:1px solid #cccccc;background-color:#ffffff'><img src='http://corporate-contacts.com/wp-content/uploads/2014/03/CorporateContactslogo3a2.png' style='max-width: 450px;margin-top: 20px;' alt='Dyn'></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc;background-color:#ffffff'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color:#ffffff'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='top' style='padding-top:20px;padding-right:30px;padding-left:30px'><table align='left' border='0' cellpadding='0' cellspacing='0'> <tbody><tr> <td valign='top' style='color:#444444;font-family:Helvetica;font-size:16px;line-height:125%;text-align:left;padding-bottom:20px'><p>Dear " + user.FullName + "</p><p>Thank you for trying Corporate Contacts. </p><p>Your 15 day trial period has now expired. We hope you tried all of the features available and have decided on the package that's right for you.  Click <a href='https://secure.corporate-contacts.com/' target='_blank'>here</a> to purchase the account you require and continue using Corporate Contacts.</p><p>Don't worry! If you decide this package is not right for you and your business you can easily select and alternative by visiting the Billings Option page and upgrade or downgrade your chosen package.</p><p>Regards,<br><a style='text-decoration:underline;color:#0066cc' href='http://corporate-contacts.com/' target='_blank'>The Corporate Contacts team</a></p><p><a href='http://www.support.corporate-contacts.com'>www.support.corporate-contacts.com</a></p></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color: #209FD1;'> <tbody><tr> <td align='center' valign='bottom'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td valign='bottom' width='600' style='padding-top:0px;padding-right:20px;padding-left:20px;padding-bottom:10px'><table align='left' border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Corporate Contacts Ltd.<br>Beverley, UK.</td><td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Phone: +44 1482 869700<br>E-Mail: hello@corporate-contacts.com</td></tr></tbody></table> </td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></center></div>"
                            };
                            notifManager.SendNotification(notifObjadmin);

                            resultsObj.accountNameList.Add(acc.AccountName);
                        }
                    }
                }
                catch (Exception ex)
                { }
            }

            return resultsObj;
        }

        public Account checkAccountTrialExpiryForAccount(Account acc)
        {            
            try
            {
                if ((acc.TrialEnds <= DateTime.Now.Date) & (acc.HasPurchased == false) & (acc.isOverFlow != false))
                {
                    bool executeStatus = CCaccountRepo.setAccountStatus(true, acc.ID);
                    acc = CCaccountRepo.Accounts.FirstOrDefault(a => a.ID == acc.ID);
                    var user = CCUserRepository.Users.FirstOrDefault(u => u.AccountID == acc.ID);
                    var notifObj = new Notification()
                    {
                        Subject = "Corporate contacts Trial End!",
                        RecipientAddress = user.Email,
                        //RecipientAddress = "brendan@davtonuk.com",
                        // Message = "Hello, \n\n" + user.FullName + " \nAccount successfully created.\nPlease click the link below to activate your account.\n\n" + Request.Url.GetLeftPart(UriPartial.Authority) + "/VerifyUserAccount/VerifyEmailAddress?GUID=" + user.GUID + "\n\nCorporate Contacts Team"
                        Message = "Dear " + user.FullName + ",\n"
                        + "This email is generated automatically by the system to notify you that your Trial Period for the registered Account '" + acc.AccountName + "' is over. Please contact one of our Support Staff "
                        + " on support@corporate-contacts.com to help you get back on track or you could simply log in back to your account and purchase a plan of your choice and cotninue enjoying with corporate-contacts.\n\n"

                        //Message = "<div><center> <table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> </tr><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td align='center' valign='top' width='600' style='padding-left:0;padding-right:0;padding-top:0;padding-bottom:0;border-top:1px solid #cccccc;border-right:1px solid #cccccc;border-left:1px solid #cccccc;background-color:#ffffff'><img src='http://corporate-contacts.com/wp-content/uploads/2014/03/CorporateContactslogo3a2.png' style='max-width: 450px;margin-top: 20px;' alt='Dyn'></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc;background-color:#ffffff'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color:#ffffff'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='top' style='padding-top:20px;padding-right:30px;padding-left:30px'><table align='left' border='0' cellpadding='0' cellspacing='0'> <tbody><tr> <td valign='top' style='color:#444444;font-family:Helvetica;font-size:16px;line-height:125%;text-align:left;padding-bottom:20px'><p>Dear " + user.FullName + "</p><p>Thank you for registering with Corporate Contacts. You have created a new account</p><p><strong>Login Email :</strong> " + user.Email + "</p><p>You still need to <a>Click Here</a> to Activate your account or Please follow the link below:</p><p><a style='text-decoration:underline;color:#0066cc' href='" + Request.Url.GetLeftPart(UriPartial.Authority) + "/VerifyUserAccount/VerifyEmailAddress?GUID=" + user.GUID + "' target='_blank'>" + Request.Url.GetLeftPart(UriPartial.Authority) + "/VerifyUserAccount/VerifyEmailAddress?GUID=" + user.GUID + "</a></p><p>If you did not sign up for this account, this will be the onlycommunication you will receive. All non-confirmed accounts areautomatically deleted after 48 hours and no addresses are kept on file. Weapologize for any inconvenience this correspondence may have caused and weassure you that it was only sent at the request of someone visiting oursite requesting an account.</p><p>If you need help, you can find it here: <a href='http://support.corporate-contacts.com'>support.corporate-contacts.com</a></p><p>Regards,<br><a style='text-decoration:underline;color:#0066cc' href='http://corporate-contacts.com/' target='_blank'>The Corporate Contacts team</a></p></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color: #209FD1;'> <tbody><tr> <td align='center' valign='bottom'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td valign='bottom' width='600' style='padding-top:0px;padding-right:20px;padding-left:20px;padding-bottom:10px'><table align='left' border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Corporate Contacts Ltd.<br>Beverley, UK, HU17 0LD<br>Manchester, NH 03101</td><td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Phone: +44 1482 869700<br>E-Mail: hello@corporate-contacts.com</td></tr></tbody></table> </td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></center></div>"
                    };
                    notifManager.SendNotification(notifObj);
                }
            }
            catch (Exception ex)
            { }

            return acc;
        }

        [HttpGet]
        public xmlClassOutput freezeAccount()
        {
            List<Account> accountList = new List<Account>();
            accountList = CCaccountRepo.Accounts.Where(a => a.isOverFlow == false).ToList();

            xmlClassOutput resultsObj = new xmlClassOutput();
            resultsObj.title = "List of Frozen Accounts";
            resultsObj.accountNameList = new List<string>();

            foreach (var acc in accountList)
            {
                try
                {
                    if (checkToFreezeAccount(acc) == 1)
                    {
                        var user = CCUserRepository.Users.FirstOrDefault(u => u.AccountID == acc.ID && u.UserType != "SystemAdmin");
                        if (user != null)
                        {
                            bool executeStatus = CCaccountRepo.setAccountStatus(true, acc.ID);
                            var notifObj = new Notification()
                            {
                                Subject = "Xobnu – account being paused",
                                RecipientAddress = user.Email,
                                Message = "<div><center> <table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> </tr><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td align='center' valign='top' width='600' style='padding-left:0;padding-right:0;padding-top:0;padding-bottom:0;border-top:1px solid #cccccc;border-right:1px solid #cccccc;border-left:1px solid #cccccc;background-color:#ffffff'><img src='http://corporate-contacts.com/wp-content/uploads/2014/03/CorporateContactslogo3a2.png' style='max-width: 450px;margin-top: 20px;' alt='Dyn'></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc;background-color:#ffffff'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color:#ffffff'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='top' style='padding-top:20px;padding-right:30px;padding-left:30px'><table align='left' border='0' cellpadding='0' cellspacing='0'> <tbody><tr> <td valign='top' style='color:#444444;font-family:Helvetica;font-size:16px;line-height:125%;text-align:left;padding-bottom:20px'><p>Dear " + user.FullName + "</p><p>Your Corporate Contacts account for " + acc.AccountName + " has been paused. This is because you have reached the limit of Connections of Items for your current plan.</p><p>To continue using the Corporate Contacts please go to our Billing Options page and upgrade your plan. Alternatively you can delete some of your contact or appointment items. However doing this may mean you do not get the full benefits of this software.</p><p>Please note this email is automatically generated. Do not reply to this email.</p><p>For assistance visit www.support.corporate-contacts.com or call +44 1482 869700.</p><p>Regards,<br><a style='text-decoration:underline;color:#0066cc' href='http://corporate-contacts.com/' target='_blank'>The Corporate Contacts team</a></p><p><a href='http://www.support.corporate-contacts.com'>www.support.corporate-contacts.com</a></p></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color: #209FD1;'> <tbody><tr> <td align='center' valign='bottom'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td valign='bottom' width='600' style='padding-top:0px;padding-right:20px;padding-left:20px;padding-bottom:10px'><table align='left' border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Corporate Contacts Ltd.<br>Beverley, UK, HU17 0LD<br>Manchester, NH 03101</td><td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Phone: +44 1482 869700<br>E-Mail: hello@corporate-contacts.com</td></tr></tbody></table> </td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></center></div>"
                            };
                            notifManager.SendNotification(notifObj);

                            resultsObj.accountNameList.Add(acc.AccountName);
                        }
                    }
                }
                catch (Exception ex)
                { }
            }

            return resultsObj;
        }

        public int checkToFreezeAccount(Account acc)
        {
            var folderList = CCFolderRepository.CCFolders.Where(f => f.AccountGUID == acc.AccountGUID).ToList();
            
            foreach(var fold in folderList)
            {
                int FolderItemCount = CCItemRepository.CCContacts.Where(i => i.FolderID == fold.FolderID).Count();

                LimitationsViewModel limitationsObj = new LimitationsViewModel();
                HelperFunctions HF = new HelperFunctions();
                limitationsObj = HF.updateAccountLimitations(acc);

                if ((FolderItemCount > limitationsObj.maxItemCountPerFolder) | (fold.isOverFlow == true))
                {
                    return 1;                    
                }
            }
            return 0;
        }

    }

    public class xmlClassOutput
    {
        public string title;
        public List<string> accountNameList;
    }
}
