using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateContacts.WebUI.Models
{
    public class AddDedupeViewModel
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
    }
}