using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Xobnu.WebUI.Models
{
    public class LayoutsViewModel
    {
        public Xobnu.Domain.Entities.CClayoutWithGroups LayoutDetails { get; set; }      
        public long ContactID { get; set; }
        public long FolderID { get; set; }
        public string FolderName { get; set; }
        public long PageID { get; set; }
        [AllowHtml]
        public string FieldValues {get; set;}
        public string FieldNames { get; set; }
        public string Note { get; set; }
        public bool IsNoteShow {get; set;}
    }
}