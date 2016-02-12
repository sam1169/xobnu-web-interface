using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xobnu.Domain.Entities;

namespace Xobnu.WebUI.Models
{
    public class ManageLayoutsAndGroupsViewModel
    {
        public List<CCLayout> Layouts { get; set; }
        public List<CCGroup> Groups { get; set; }
        public string FolderName { get; set; }
    }
}