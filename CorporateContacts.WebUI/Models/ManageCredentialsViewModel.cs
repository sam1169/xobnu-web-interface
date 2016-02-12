using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xobnu.Domain.Entities;

namespace Xobnu.WebUI.Models
{
    public class ManageCredentialsViewModel
    {
        public List<Credential> Credentials { get; set; }
    }
}