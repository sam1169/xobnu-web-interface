using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateContacts.Domain.Entities
{
    [Table("tblHistory")]
    public class CCHistoryLog
    {
        [Key]
        public long ID { get; set; }
        public string AccountGUID { get; set; }
        public string Source { get; set; }
        public string Action { get; set; }
        public long ConnectionID { get; set; }
        public long ItemID { get; set; }
        public long FieldID { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime Date { get; set; }
    }
}
