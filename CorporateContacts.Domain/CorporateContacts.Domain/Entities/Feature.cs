using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateContacts.Domain.Entities
{
    [Table("tblFeatures")]
    public class Feature
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
        public int PlanLevel { get; set; }
        public bool Visible { get; set; }
        public decimal Price { get; set; }
        public string StripePlanID { get; set; }
    }
}
