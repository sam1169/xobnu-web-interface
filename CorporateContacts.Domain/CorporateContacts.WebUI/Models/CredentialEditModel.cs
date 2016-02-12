using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.WebUI.Models
{
    public class CredentialEditModel
    {
        public Credential CredentialDetails { get; set; }
        public string ButtonStatus { get; set; }
    }
}