using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Xobnu.Domain.Entities
{
    [Table("tblGroupFields")]
   public class CCGroupField
    {
        [Key]
        public long GrpFieldID { get; set; }
        public long GroupID { get; set; }
        public long FieldID { get; set; }
        public long FolderID { get; set; }
        public string AccountGUID { get; set; }
        public long FieldOrder { get; set; }

    }
}
