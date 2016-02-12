using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.WebUI.Models
{
    public class LimitationsViewModel
    {
        public int availableCconnectionCount { get; set; }
        public int purchasedConnectionCount { get; set; }
        public int maxItemCountPerFolder { get; set; }
        public Boolean isCalendarSyncAvailable { get; set; }
        public List<FolderDetailsWithItemCount> folderList { get; set; }
    }

    public class FolderDetailsWithItemCount
    {
        public CCFolder fold { get; set; }
        public long itemCount { get; set; }
    }
}