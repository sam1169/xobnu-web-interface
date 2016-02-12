using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xobnu.Domain.Entities
{
    public class SubscriptionDto 
    {
        public long ID { get; set; }
        public long AccountID { get; set; }
        public long SubscriberID { get; set; }
        public long PrimaryID { get; set; }
        public string PrimaryName { get; set; }
        public bool TwoWay { get; set; }
        public bool AllowAdditions { get; set; }
        public bool IgnoreExisting { get; set; }
        public DateTime? LastSync { get; set; }
        public string Status { get; set; }
        public Folder SubscribingFolder { get; set; }
    }
}
