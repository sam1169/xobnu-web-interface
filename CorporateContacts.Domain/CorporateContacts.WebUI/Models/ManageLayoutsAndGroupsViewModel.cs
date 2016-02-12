using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.WebUI.Models
{
    public class ManageLayoutsAndGroupsViewModel
    {
        public List<CCLayout> Layouts { get; set; }
        public List<CCGroup> Groups { get; set; }
        public string FolderName { get; set; }
    }
}