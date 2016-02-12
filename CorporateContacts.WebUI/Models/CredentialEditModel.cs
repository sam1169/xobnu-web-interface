using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xobnu.Domain.Entities;

namespace Xobnu.WebUI.Models
{
    public class CredentialEditModel
    {
        public Credential CredentialDetails { get; set; }
        public string ButtonStatus { get; set; }
    }
}