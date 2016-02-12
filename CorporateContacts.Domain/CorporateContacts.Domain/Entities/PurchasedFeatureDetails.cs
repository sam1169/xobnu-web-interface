using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateContacts.Domain.Entities
{
    public class PurchasedFeatureDetails
    {
        public string PlanName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public DateTime Expiry { get; set; }
    }
}