using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Xobnu.Domain.Entities
{
     [Table("tblFolders")]
    public class CCFolder
    {  
        [Key]
        public long FolderID { get; set; }
        [Required(ErrorMessage = "Please enter a folder name")]
        public string Name { get; set; }
        public string AccountGUID { get; set; }
        public Int16 Type { get; set; }
        public bool? IsPaused { get; set; }
        public bool? isOverFlow { get; set; }
        public bool? isCrimeDiary { get; set; }

        public CCFolder()
        {
            isCrimeDiary = false;
        }
    }
}
