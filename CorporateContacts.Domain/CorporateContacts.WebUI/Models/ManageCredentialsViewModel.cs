using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.WebUI.Models
{
    public class ManageCredentialsViewModel
    {
        public List<Credential> Credentials { get; set; }
    }
}