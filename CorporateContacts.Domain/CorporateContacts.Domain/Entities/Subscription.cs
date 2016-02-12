using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateContacts.Domain.Entities
{
    [Table("tblSubscriptions")]
    public class Subscription
    {
        public long ID { get; set; }
        public long AccountID { get; set; }
        public long SubscriberID { get; set; }
        public long PrimaryID { get; set; }
        public bool TwoWay { get; set; }
        public bool AllowAdditions { get; set; }
        public bool IgnoreExisting { get; set; }
        public DateTime? LastSync { get; set; }
        public string Status { get; set; }
    }
}
