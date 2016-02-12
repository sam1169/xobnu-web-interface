﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Xobnu.WebUI.Models
{
    public class SubscriptionsViewModel
    {
        public IEnumerable<Xobnu.Domain.Entities.CCConnection> SubscriptionDetails { get; set; }
        public Xobnu.Domain.Entities.CCConnection Subscriptiondetail { get; set; }
        public string FolderName { get; set; }
        public long FolderID { get; set; }
        public int FolderType { get; set; }
        public List<string> CredentialName { get; set; }
        public ConnectionImportListSummaryModel conSummary { get; set; }
        public LimitationsViewModel limitationsObj { get; set; }
    }
}