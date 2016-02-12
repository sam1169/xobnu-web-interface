using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.WebUI.Models
{
    public class AdminDashboardViewModel
    {       
        public string FolderName { get; set; }
        public int FolderType { get; set; }
        public long NumberOfItems { get; set; }
        public long NumberOfConnections { get; set; }
        public long FolderID { get; set; }
        public bool IsPaused { get; set; }
        
    }
}