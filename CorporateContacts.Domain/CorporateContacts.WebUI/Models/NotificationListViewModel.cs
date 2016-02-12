using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateContacts.WebUI.Models
{
    public class NotificationListViewModel
    {
        public string NotificationMsg { get; set; }
        public string url { get; set; }
        public long priority { get; set; }
        public string NotificationType { get; set; }

    }
}