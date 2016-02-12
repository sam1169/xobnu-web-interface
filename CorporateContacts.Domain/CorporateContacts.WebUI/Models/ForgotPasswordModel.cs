using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorporateContacts.WebUI.Models
{
    public class ForgotPasswordModel
    {
        public string EmailAddress { get; set; }
        
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [System.Web.Mvc.Compare("Password")]
        [NotMapped]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public long AccountID { get; set; }
        
    }
}