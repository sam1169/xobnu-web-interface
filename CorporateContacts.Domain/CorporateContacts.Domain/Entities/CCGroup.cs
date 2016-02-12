using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CorporateContacts.Domain.Entities
{
     [Table("tblGroups")]
    public class CCGroup
    {
         [Key]
         public long GroupID { get; set; }
         [Required(ErrorMessage = "Please enter group name")]
         public string GroupName { get; set; }
         public long FolderID { get; set; }
         public string AccountGUID { get; set; }

    }
}
