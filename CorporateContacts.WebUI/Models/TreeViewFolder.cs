using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Xobnu.WebUI.Models
{
    public class TreeViewFolder
    {
        public TreeViewFolder()
        {
            ChildFolders = new HashSet<TreeViewFolder>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string FolderClass { get; set; }
        public ICollection<TreeViewFolder> ChildFolders { get; set; }
    }
}