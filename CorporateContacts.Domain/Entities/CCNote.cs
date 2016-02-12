using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Xobnu.Domain.Entities
{
    [Table("tblNotes")]
    public class CCNote
    {
        [Key]
        public long NotesID { get; set; }
        public long ContactID { get; set; }
        public string value { get; set; }
        public string AccountGUID { get; set; }
    }
}
