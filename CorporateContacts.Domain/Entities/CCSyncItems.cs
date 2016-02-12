using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Xobnu.Domain.Entities
{
    [Table("tblSyncItems")]
    public class CCSyncItems
    {
        [Key]
        public long ID { get; set; }
        public long ConnectionID { get; set; }    
        public string RefID { get; set; }      
        public string Tag { get; set; }
        public bool isRecurring { get; set; } 
        public bool isDistGroup { get; set; }
        public long? MappedItemID { get; set; }
        public bool isDeleted { get; set; }       
        public DateTime? LastUpdated { get; set; }      
        public string DeDupeValue { get; set; }
        public string TextBody { get; set; }
        public string Notes { get; set; }
        public string RecurringMasterID { get; set; }
        public int RecurrenceNo { get; set; }
        public bool HasPicture { get; set; }
        public long HashCode { get; set; }
        public long PicID { get; set; }
    }
}
