using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Xobnu.WebUI.Models.jsonmodels
{
   

    public class ObjectT
    {
        public string @object { get; set; }
        public string id { get; set; }
        public int date { get; set; }
        public int amount { get; set; }
        public bool livemode { get; set; }
        public bool proration { get; set; }
        public string currency { get; set; }
        public string customer { get; set; }
        public string description { get; set; }
       // public Metadata metadata { get; set; }
        public object invoice { get; set; }
        public string subscription { get; set; }
    }

    public class DataT
    {
        public Object @object { get; set; }
    }

    public class InvoiceCreatedRequest
    {
        public int created { get; set; }
        public bool livemode { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string @object { get; set; }
        public object request { get; set; }
        public Data data { get; set; }
    }
}