using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xobnu.Domain.Entities;

namespace Xobnu.WebUI.Models
{
    public class AddContactViewModel
    {
        public List<CCFolderField> FolderFields { get; set; }
        public string FolderValues { get; set; }
        public long FolderID { get; set; }
        public int FolderType { get; set; }
        public bool isCrimeDiary { get; set; }
    }
}