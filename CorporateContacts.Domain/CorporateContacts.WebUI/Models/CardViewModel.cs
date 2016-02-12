using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateContacts.WebUI.Models
{
    public class CardViewModel
    {
        public string Name { get; set;}
        public string Type { get; set; }
        public string Number { get; set; }
        public string ExpireDate { get; set; }
        public string CardID { get; set; }

    }
}