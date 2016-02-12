using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateContacts.WebUI.Models
{
    public class AdminViewModel
    {
        public List<AdminDashboardViewModel> FoldersInfor { get; set; }
        public List<List<ConnectionInforViewModel>> ConnectionsInfor { get; set; }

        public long noOfConnections { get; set; }
        public int NoOfConnection { get; set; }
        public long noOfFolders { get; set; }
        public long noOfUsers { get; set; }
        public long noOfSubscriptionsPurchased { get; set; }
        public long noOfTotalItems { get; set; }
    }
}