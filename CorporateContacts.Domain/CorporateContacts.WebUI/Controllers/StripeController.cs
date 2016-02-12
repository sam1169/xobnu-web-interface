using CorporateContacts.WebUI.Models.jsonmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Stripe;
using CorporateContacts.Domain.Entities;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Concrete;

namespace CorporateContacts.WebUI.Controllers
{
    public class StripeController : ApiController
    {
        IAccountRepo CCaccountRepo;
        INotificationManager notifManager;
        IUserRepo CCUserRepository;
        //
        // GET: /Stripe/

        //public HttpResponseMessage Index()
        //{
        //    //return View();
        //    return new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK };
        //}

        //public int test2(string tt,InvoiceCreatedRequest request)
        //{
        //    var ttt = request;
        //    return 1;
        //    //return Json("Sucess", JsonRequestBehavior.AllowGet);
        //}

        public StripeController()
        {
            this.notifManager = new EmailNotificationManager();
            this.CCaccountRepo = new EFAccountRepo();
            CCUserRepository = new EFUserRepo();
        }

        public StripeController(INotificationManager notifMgr, IAccountRepo accRepo, IUserRepo urep)
        {
            notifManager = notifMgr;
            CCUserRepository = urep;
            CCaccountRepo = accRepo;
        }

        public string InvoiceCreated(InvoiceCreateViewModel request)
        {
            if (request.type == "invoice.created")
            {
                try
                {
                    var invoi = new StripeInvoiceItemService();
                    invoi.ApiKey = "sk_test_4Xusc3Meo8gniONh6dDRZvlp";
                    var invopt = new StripeInvoiceItemCreateOptions();
                    invopt.Amount = request.data.@object.total;
                    invopt.Currency = request.data.@object.currency; 
                    invopt.InvoiceId = request.data.@object.id;
                    invopt.CustomerId = request.data.@object.customer;
                    var resInvoice = invoi.Create(invopt);
                }
                catch (StripeException ex)
                {
                    return "Error" + ex.Message;
                }
            }

            return "1";
            //return Json("Sucess", JsonRequestBehavior.AllowGet);
        }

        public string ChargeFailed(ChargeFailedViewModel request)
        {
            //Occurs whenever a failed charge attempt occurs.
            if (request.type == "charge.failed")
            {
                var accountDetails = new Account();
                var user = new User();

                if (request.data.@object.customer != null)
                {
                    accountDetails = CCaccountRepo.Accounts.Where(a => a.StripeCustomerID == request.data.@object.customer).First();
                    user = CCUserRepository.Users.FirstOrDefault(u => u.AccountID == accountDetails.ID);
                }
                else
                {
                    user = CCUserRepository.Users.FirstOrDefault(u => u.Email == request.data.@object.card.name);
                    accountDetails = CCaccountRepo.Accounts.Where(a => a.ID == user.AccountID).First();                   
                }
                
                try
                {

                    if ((accountDetails.TrialEnds <= DateTime.Now.Date) & (accountDetails.HasPurchased != false))
                    {

                        accountDetails.isPaymentIssue = true;
                        CCaccountRepo.SaveAccount(accountDetails);
                    }
                        var notifObj = new Notification()
                        {
                            Subject = "Corporate Contacts - Payment Failure",
                            RecipientAddress = user.Email,
                            Message = "<div><center> <table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> </tr><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td align='center' valign='top' width='600' style='padding-left:0;padding-right:0;padding-top:0;padding-bottom:0;border-top:1px solid #cccccc;border-right:1px solid #cccccc;border-left:1px solid #cccccc;background-color:#ffffff'><img src='http://corporate-contacts.com/wp-content/uploads/2014/03/CorporateContactslogo3a2.png' style='max-width: 450px;margin-top: 20px;' alt='Dyn'></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc;background-color:#ffffff'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color:#ffffff'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='top' style='padding-top:20px;padding-right:30px;padding-left:30px'><table align='left' border='0' cellpadding='0' cellspacing='0'> <tbody><tr> <td valign='top' style='color:#444444;font-family:Helvetica;font-size:16px;line-height:125%;text-align:left;padding-bottom:20px'><p>Dear " + user.FullName + "</p><p>We had trouble charging $" + ((request.data.@object.amount).ToString()).Substring(0, (((request.data.@object.amount).ToString()).Length - 2)) + "." + ((request.data.@object.amount).ToString()).Substring(((request.data.@object.amount).ToString()).Length - 2) + " from your card **** " + request.data.@object.source.last4 + " </p><p>The bank replied with the following details and we would like to share with you in order for you to update the card details accordingly.</p><p>" + request.data.@object.failure_message + "</p><p>Regards,<br><a style='text-decoration:underline;color:#0066cc' href='http://corporate-contacts.com/' target='_blank'>The Corporate Contacts team</a></p><p><a href='http://www.support.corporate-contacts.com'>www.support.corporate-contacts.com</a></p></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color: #209FD1;'> <tbody><tr> <td align='center' valign='bottom'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td valign='bottom' width='600' style='padding-top:0px;padding-right:20px;padding-left:20px;padding-bottom:10px'><table align='left' border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Corporate Contacts Ltd.<br>Beverley, UK, HU17 0LD<br>Manchester, NH 03101</td><td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Phone: +44 1482 869700<br>E-Mail: hello@corporate-contacts.com</td></tr></tbody></table> </td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></center></div>"
                            //Message = "Test Email <br/> " + request.data.@object.customer + "<br/>" + request.data.@object.id + "<br/>" + request.data.@object.amount + "<br/>" + request.data.@object.failure_message
                        };
                        notifManager.SendNotification(notifObj);      

                }
                catch (StripeException ex)
                {
                    return request.data.@object.amount + "Error" + ex.Message;
                }

                return "1-" + accountDetails.AccountName + " / " + user.Email;
            }
            else
            {
                return "0";
            }        
        }

        public string ChargeCaptured(ChargeFailedViewModel request)
        {
            //Occurs whenever a previously uncaptured charge is captured.
            if (request.type == "charge.captured")
            {
                var accountDetails = CCaccountRepo.Accounts.Where(a => a.StripeCustomerID == request.data.@object.customer).First();
                var user = CCUserRepository.Users.FirstOrDefault(u => u.AccountID == accountDetails.ID);

                try
                {
                    if (accountDetails.isPaymentIssue == true)
                    {
                        accountDetails.isPaymentIssue = false;
                        CCaccountRepo.SaveAccount(accountDetails);
                    }

                    //Send an email which says payment processed succesfully.

                    var notifObj = new Notification()
                    {
                        Subject = "Corporate Contacts - Payment Updated (testing)",
                        RecipientAddress = user.Email,
                        Message = "<div><center> <table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> </tr><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td align='center' valign='top' width='600' style='padding-left:0;padding-right:0;padding-top:0;padding-bottom:0;border-top:1px solid #cccccc;border-right:1px solid #cccccc;border-left:1px solid #cccccc;background-color:#ffffff'><img src='http://corporate-contacts.com/wp-content/uploads/2014/03/CorporateContactslogo3a2.png' style='max-width: 450px;margin-top: 20px;' alt='Dyn'></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc;background-color:#ffffff'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color:#ffffff'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='top' style='padding-top:20px;padding-right:30px;padding-left:30px'><table align='left' border='0' cellpadding='0' cellspacing='0'> <tbody><tr> <td valign='top' style='color:#444444;font-family:Helvetica;font-size:16px;line-height:125%;text-align:left;padding-bottom:20px'><p>Dear " + user.FullName + "</p><p>We had trouble charging $" + ((request.data.@object.amount).ToString()).Substring(0, (((request.data.@object.amount).ToString()).Length - 2)) + "." + ((request.data.@object.amount).ToString()).Substring(((request.data.@object.amount).ToString()).Length - 2) + " from your card **** " + request.data.@object.source.last4 + " </p><p>The bank replied with the following details and we would like to share with you in order for you to update the card details accordingly.</p><p>" + request.data.@object.failure_message + "</p><p>Regards,<br><a style='text-decoration:underline;color:#0066cc' href='http://corporate-contacts.com/' target='_blank'>The Corporate Contacts team</a></p><p><a href='http://www.support.corporate-contacts.com'>www.support.corporate-contacts.com</a></p></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color: #209FD1;'> <tbody><tr> <td align='center' valign='bottom'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td valign='bottom' width='600' style='padding-top:0px;padding-right:20px;padding-left:20px;padding-bottom:10px'><table align='left' border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Corporate Contacts Ltd.<br>Beverley, UK, HU17 0LD<br>Manchester, NH 03101</td><td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Phone: +44 1482 869700<br>E-Mail: hello@corporate-contacts.com</td></tr></tbody></table> </td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></center></div>"
                        //Message = "Test Email <br/> " + request.data.@object.customer + "<br/>" + request.data.@object.id + "<br/>" + request.data.@object.amount + "<br/>" + request.data.@object.failure_message
                    };
                    notifManager.SendNotification(notifObj);

                }
                catch (StripeException ex)
                {
                    return request.data.@object.amount + "Error" + ex.Message;
                }

                return "1-" + accountDetails.AccountName + " / " + user.Email;
            }
            else
            {
                return "0";
            }
        }

        public string Chargesucceeded(ChargeFailedViewModel request)
        {
            //Occurs whenever a previously uncaptured charge is captured.
            if (request.type == "charge.succeeded")
            {
                var accountDetails = CCaccountRepo.Accounts.Where(a => a.StripeCustomerID == request.data.@object.customer).First();
                var user = CCUserRepository.Users.FirstOrDefault(u => u.AccountID == accountDetails.ID);
                try
                {
                    if (accountDetails.isPaymentIssue == true | accountDetails.HasPurchased == false)
                    {
                        accountDetails.isPaymentIssue = false;
                        accountDetails.HasPurchased = true;
                        CCaccountRepo.SaveAccount(accountDetails);


                        //Send an email which says payment processed succesfully.

                        var notifObj = new Notification()
                        {
                            Subject = "Corporate Contacts - Payment Processed Successfully (testing)",
                            RecipientAddress = user.Email,
                            Message = "<div><center> <table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> </tr><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td align='center' valign='top' width='600' style='padding-left:0;padding-right:0;padding-top:0;padding-bottom:0;border-top:1px solid #cccccc;border-right:1px solid #cccccc;border-left:1px solid #cccccc;background-color:#ffffff'><img src='http://corporate-contacts.com/wp-content/uploads/2014/03/CorporateContactslogo3a2.png' style='max-width: 450px;margin-top: 20px;' alt='Dyn'></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc;background-color:#ffffff'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color:#ffffff'> <tbody><tr> <td align='center' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='top' style='padding-top:20px;padding-right:30px;padding-left:30px'><table align='left' border='0' cellpadding='0' cellspacing='0'> <tbody><tr> <td valign='top' style='color:#444444;font-family:Helvetica;font-size:16px;line-height:125%;text-align:left;padding-bottom:20px'><p>Dear " + user.FullName + "</p><p>We had trouble charging $" + ((request.data.@object.amount).ToString()).Substring(0, (((request.data.@object.amount).ToString()).Length - 2)) + "." + ((request.data.@object.amount).ToString()).Substring(((request.data.@object.amount).ToString()).Length - 2) + " from your card **** " + request.data.@object.source.last4 + " </p><p>The bank replied with the following details and we would like to share with you in order for you to update the card details accordingly.</p><p>" + request.data.@object.failure_message + "</p><p>Regards,<br><a style='text-decoration:underline;color:#0066cc' href='http://corporate-contacts.com/' target='_blank'>The Corporate Contacts team</a></p><p><a href='http://www.support.corporate-contacts.com'>www.support.corporate-contacts.com</a></p></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr><tr> <td align='center' valign='top' style='border-left:1px solid #cccccc;border-right:1px solid #cccccc'><table border='0' cellpadding='0' cellspacing='0' width='100%' style='background-color: #209FD1;'> <tbody><tr> <td align='center' valign='bottom'><table border='0' cellpadding='0' cellspacing='0' width='600'> <tbody><tr> <td valign='bottom' width='600' style='padding-top:0px;padding-right:20px;padding-left:20px;padding-bottom:10px'><table align='left' border='0' cellpadding='0' cellspacing='0' width='100%'> <tbody><tr> <td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Corporate Contacts Ltd.<br>Beverley, UK, HU17 0LD<br>Manchester, NH 03101</td><td valign='bottom' style='color:#ffffff;padding-top:10px;font-family:Helvetica;font-size:12px;line-height:150%;text-align:left'>Phone: +44 1482 869700<br>E-Mail: hello@corporate-contacts.com</td></tr></tbody></table> </td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></center></div>"
                            //Message = "Test Email <br/> " + request.data.@object.customer + "<br/>" + request.data.@object.id + "<br/>" + request.data.@object.amount + "<br/>" + request.data.@object.failure_message
                        };
                        notifManager.SendNotification(notifObj);
                    }
                }
                catch (StripeException ex)
                {
                    return request.data.@object.amount + "Error" + ex.Message;
                }

                return "1-" + accountDetails.AccountName + " / " + user.Email;
            }
            else
            {
                return "0";
            }
        }

    }
}
