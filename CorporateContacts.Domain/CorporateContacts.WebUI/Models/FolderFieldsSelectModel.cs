using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateContacts.WebUI.Models
{
    public class FolderFieldsSelectModel
    {
        public IEnumerable<String> SelectFolderFieldsSchemas { get; set; }
        public long FolderID { get; set; }
        public List<string> SavedFields { get; set; }
        public int Type { get; set; }
    }
}