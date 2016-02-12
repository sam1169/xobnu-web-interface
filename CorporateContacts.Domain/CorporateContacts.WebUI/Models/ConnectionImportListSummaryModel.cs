using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateContacts.WebUI.Models
{
    public class ConnectionImportListSummaryModel
    {
        public int NoOfSuccessfullConnections { get; set; }
        public List<string> UnsuccessfulEmailList { get; set; }
        public Boolean maxConnectionLevelReach { get; set; }

        public ConnectionImportListSummaryModel()
        {
            maxConnectionLevelReach = false;
            UnsuccessfulEmailList = new List<string>();
        }

    }
}