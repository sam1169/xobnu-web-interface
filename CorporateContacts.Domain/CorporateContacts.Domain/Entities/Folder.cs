using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateContacts.Domain.Entities
{
    [Table("tblFolders")]
    public class Folder
    {
        public long ID { get; set; }
        public bool IsMaster { get; set; }
        public string FolderName { get; set; }
        public string FolderType { get; set; }
        public string SourceName { get; set; }
        public string SourceType { get; set; }
        public string SourceID { get; set; }
        public string SourceID2 { get; set; }
        public string Description { get; set; }
        public long AccountID { get; set; } 


    }
}
