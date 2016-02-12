using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xobnu.Domain.Entities;

namespace Xobnu.WebUI.Models
{
    public class ImportConnectionListModel
    {
        public List<Credential> ExistingCredentials { get; set; }
        public long SelectedCredentialID { get; set; }
        public Credential Credentials { get; set; }
        public long selectedFolderID { get; set; }
        public List<AccessMethodForExchange> ImporsanationOrDelegation { get; set; }
        public string SelectedImporsanationOrDelegation { get; set; }
        public string ItemType { get; set; }
        public string SyncDirection { get; set; }

        public ImportConnectionListModel()
        {
            ImporsanationOrDelegation = new List<AccessMethodForExchange>();


            AccessMethodForExchange A1 = new AccessMethodForExchange();
            A1.ID = 1;
            A1.Name = "Impersonation";
            ImporsanationOrDelegation.Add(A1);

            AccessMethodForExchange A2 = new AccessMethodForExchange();
            A2.ID = 2;
            A2.Name = "Delegation";
            ImporsanationOrDelegation.Add(A2);

        }

    }

    public class AccessMethodForExchange
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}