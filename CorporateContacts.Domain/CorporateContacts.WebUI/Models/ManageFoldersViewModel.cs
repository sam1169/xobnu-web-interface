using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.WebUI.Models
{
    public class ManageFoldersViewModel
    {
        public List<CCFolder> Folders { get; set; }
        public List<Credential> Credentials { get; set; }
    }
}