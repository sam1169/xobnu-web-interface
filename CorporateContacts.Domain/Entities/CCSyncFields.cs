using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Xobnu.Domain.Entities
{
    [Table("tblSyncFields")]
    public class CCSyncFields
    {
        [Key]
        public long ID { get; set; }
        public long SyncItemID { get; set; }    
        public string Name { get; set; }
        public string Value { get; set; }     
        public string Type { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
