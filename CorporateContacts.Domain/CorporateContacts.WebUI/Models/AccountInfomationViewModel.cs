using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.WebUI.Models
{
    public class AccountInfomationViewModel
    {
        public string AccountName { get; set; }
        public string ContactName { get; set; }
        public string BillingPlan { get; set; }
        public int NoOfConnection { get; set; }
        public string BillingAddress { get; set; }
        public string TelephoneNo { get; set; }
        public string EmailAddress { get; set; }
        public string TimeZone { get; set; }
        public List<TimeZoneViewModel> TimeZoneList { get; set; }

        public long noOfConnections { get; set; }
        public long noOfFolders { get; set; }
        public long noOfUsers { get; set; }
        public long noOfSubscriptionsPurchased { get; set; }
        public long noOfTotalItems { get; set; }

        public List<Plan> Plans { get; set; }
        public int PlanID { get; set; }
        public int QuantitySaved { get; set; }

        public List<User> ListofUsers { get; set; }
        public List<CCErrorLog> ErrorLogList { get; set; }

        public string activeTab { get; set; }
    }
}