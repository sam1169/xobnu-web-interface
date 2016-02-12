using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CorporateContacts.Domain.Entities
{
    [Table("tblLayouts")]
    public class CCLayout
    {
        [Key]
        public long LayoutID { get; set; }
        [Required(ErrorMessage = "Please enter layout name")]
        public string LayoutName { get; set; }
        public long FolderID { get; set; }
        public string AccountGUID { get; set; }
    }
}
