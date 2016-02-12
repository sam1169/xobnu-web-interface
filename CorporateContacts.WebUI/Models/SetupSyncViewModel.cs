using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xobnu.Domain.Entities;

namespace Xobnu.WebUI.Models
{
    public class SetupSyncViewModel
    {
        public List<Folder> MasterSources { get; set; }
        public List<SubscriptionDto> SubscriptionsForCurrentMaster { get; set; }
        public long CurrentMasterID { get; set; }
        public string CurrentMasterFolderName { get; set; }
    }
}