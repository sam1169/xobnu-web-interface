using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xobnu.Domain.Entities
{
    [Table("tblPurchasedFeatures")]
    public class PurchasedFeatures
    {
        public long ID { get; set; }
        public string AccountGUID { get; set; }
        public long FeatureID { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool Enabled { get; set; }
        public int Quantity { get; set; }

    }
}
