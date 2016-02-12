using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Xobnu.WebUI.Models
{
    public class LayoutGroupsViewModel
    {
        public IEnumerable<String> AllGroups { get; set; }
        public IEnumerable<String> SavedGroups { get; set; }
        public long FolderID { get; set; }
        public long LayoutID { get; set; }
        public bool IsSave { get; set; }
    }
}