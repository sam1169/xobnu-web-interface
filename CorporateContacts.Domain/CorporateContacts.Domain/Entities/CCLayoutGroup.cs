using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CorporateContacts.Domain.Entities
{
    [Table("tblLayoutGroups")]
    public class CCLayoutGroup
    {
        [Key]
        public long LayoutGrpID { get; set; }
        public long LayoutID { get; set; }
        public long GroupID { get; set; }
        public long FolderID { get; set; }
        public string AccountGUID { get; set; }
        public long GroupOrder { get; set; }
    }
}
