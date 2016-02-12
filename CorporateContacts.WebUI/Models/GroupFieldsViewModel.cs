using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Xobnu.WebUI.Models
{
    public class GroupFieldsViewModel
    {
        public IEnumerable<String> AllFields { get; set; }
        public IEnumerable<String> SavedFields { get; set; }
        public long FolderID { get; set; }
        public long GroupID { get; set; }
        public bool IsSave { get; set; }
    }
}