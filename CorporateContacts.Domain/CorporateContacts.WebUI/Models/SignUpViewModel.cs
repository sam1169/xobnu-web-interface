using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.WebUI.Models
{
    public class SignUpViewModel
    {
        public Account Account { get; set; }
        public User User { get; set; }
        public List<TimeZoneViewModel> TimeZoneList { get; set; }
    }
}