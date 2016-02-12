using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Xobnu.Domain.Entities
{
    [Table("tblFieldValues")]
    public class CCFieldValue
    {
        [Key]
        public long ValueID { get; set; }
        public long ItemID { get; set; }
        public long FieldID { get; set; }
        public string Value { get; set; }
        public string AccountGUID { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Value2 { get; set; }
    }
}
