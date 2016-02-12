using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xobnu.Domain.Entities;

namespace Xobnu.WebUI.Models
{
    public class ItemsViewModel
    {
        public CCFolder FolderDetails { get; set; }
        public bool IsCrimeDiaryFields { get; set; }
        public LimitationsViewModel limitationsObj { get; set; }
    }
}