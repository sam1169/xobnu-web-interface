using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xobnu.Domain.Entities;

namespace Xobnu.WebUI.Models
{
    public class ImpersonateAccountsSysAdminModel
    {
        public List<AccountDetails> listOfAccounts { get; set; }
        public long FolderCount { get; set; }
        public long ConnectionCount { get; set; }
        public long UserCount { get; set; }
        public long SubscriptionCount { get; set; }
        public long ItemsCount { get; set; }
        public Account SelectedAccountInfo { get; set; }
        public string SelectedAccountOwner { get; set; }
        public List<FolderListDetails> SelectedAccountFolderList { get; set; }
        public CCFolder SelectedFolderInfo { get; set; }
        public List<CCItems> SelectedFolderItemList { get; set; }

        public List<string> SyncDates { get; set; }
        public List<double> SyncDateUsage { get; set; }



        public ImpersonateAccountsSysAdminModel()
        {
            listOfAccounts = new List<AccountDetails>();
            SyncDates = new List<string>();
            SyncDateUsage = new List<double>();
        }
    }

    public class AccountDetails
    {
        public long AccountID { get; set; }
        public string AccountGUID { get; set; }
        public string AccountName { get; set; }
        public string Email { get; set; }
        public string DateCreated { get; set; }
        public string Usgae { get; set; }
    }

    public class FolderListDetails
    {
        public CCFolder FolderDetails { get; set; }
        public long ItemCount { get; set; }
    }
}