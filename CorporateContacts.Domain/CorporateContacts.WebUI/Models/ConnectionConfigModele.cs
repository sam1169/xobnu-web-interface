using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;

namespace CorporateContacts.WebUI.Models
{
    public class ConnectionConfig
    {
        public ExchangeVersion version { get; set; }
        public string url { get; set; }
    }

    public class ConnectionConfigModele
    {
    }

}