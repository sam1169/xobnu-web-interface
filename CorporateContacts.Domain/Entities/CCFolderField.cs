using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Xobnu.Domain.Entities
{
     [Table("tblFolderFields")]
   public class CCFolderField
    {
         [Key]
         public long  FieldID { get; set; }
         [Required(ErrorMessage = "Please enter folder field name")]
         public string FieldName { get; set; }
         public string FieldType { get; set; }
         public long FolderID { get; set; }
         public string AccountGUID { get; set; }
         public string FieldCaption { get; set; }
         public string Restriction { get; set; }
         public string RestrictionValues { get; set; }
         public bool isActive { get; set; }

         [NotMapped]
         public bool isMandatory { get; set; }

         public CCFolderField()
         {
             isMandatory = false;
         }
    }
}
