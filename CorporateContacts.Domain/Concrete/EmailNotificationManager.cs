using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Concrete
{
    public class EmailSettings
    {
        public string MailFromAddress = "Corporate Contacts<hello@corporate-contacts.com>";
        public bool UseSsl = true;
        public string Username = "david@corporate-contacts.com";
        public string Password = "oH-zcBzI1Nh7gtAlELQjGw";
        public string ServerName = "smtp.mandrillapp.com";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation = @"c:\sports_store_emails";
    }


    public class EmailNotificationManager : INotificationManager
    {
        private EmailSettings emailSettings;
        public EmailNotificationManager()
        {
            emailSettings = new EmailSettings();
        }
        public EmailNotificationManager(EmailSettings emailSet)
        {
            emailSettings = emailSet;
        }
        public void SendNotification(Notification notifObj)
        {

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = false;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);
                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }
                MailMessage mailMessage = new MailMessage(emailSettings.MailFromAddress, notifObj.RecipientAddress, notifObj.Subject, notifObj.Message);
                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }
                mailMessage.IsBodyHtml = true;
                smtpClient.Send(mailMessage);
            }
        }
    }
}
