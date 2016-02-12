using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Xobnu.Domain.Entities
{
    [Table("tblItems")]
    public class CCItems
    {
        [Key]
        public long ItemID { get; set; }     
        public long FolderID { get; set; }      
        public DateTime LastUpdated { get; set; }
        public bool isRecurring { get; set; }
        public bool isDistGroup { get; set; }
        public bool isDeleted { get; set; }
        public DateTime Created { get; set; }       
        public string DeDupeValue { get; set; }
        public string TextBody { get; set; }
        public string Notes { get; set; }
        public bool HasPicture { get; set; }
        public long HashCode { get; set; }
        public long PicID { get; set; }
        public string AccountGUID { get; set; }
    }
}
