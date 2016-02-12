using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateContacts.WebUI.Models
{
    public class ConnectionInforViewModel
    {
        public string FolderName { get; set; }
        public long ConnectionID { get; set; }
        public string ConnctionFolderName { get; set; }
        public string Type { get; set; }
        public string LastSyncTime { get; set; }       
        public bool IsRunning { get; set; }
        public long FolderID { get; set; }
        public bool IsPaused { get; set; }
        public string rowClass { get; set; }
    }
}