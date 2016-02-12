using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xobnu.Domain.Entities;

namespace Xobnu.WebUI.Models
{
    public class ManageFoldersViewModel
    {
        public List<CCFolder> Folders { get; set; }
        public List<Credential> Credentials { get; set; }
    }
}