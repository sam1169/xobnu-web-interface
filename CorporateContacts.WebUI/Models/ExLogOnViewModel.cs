using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xobnu.Domain.Entities;

namespace Xobnu.WebUI.Models
{
    public class ExLogOnViewModel
    {
        public Credential Credentials { get; set; }
        public string ReturnUrl { get; set; }
        public List<Credential> ExistingCredentials { get; set; }
        public long SelectedCredentialID { get; set; }
        public string SelectedButton { get; set; }
        public long selectedFolderID { get; set; }
        public string ServerVer { get; set; }
        public CCConnection Connection { get; set; }
        public string SecondaryAccount { get; set; }
        public int AccessType { get; set; }
        public string SecondaryAccountc { get; set; }
        public string SecondaryAccountIn { get; set; }
        public int AccessTypec { get; set; }
        public int PlanID { get; set; }
        public string SpecificURL { get; set; }
        public int ExchangeType { get; set; }
        public string SelectedButtonInhouse { get; set; }
        [Required(ErrorMessage = "Please enter a name")]
        public string Name { get; set; }
         [Required(ErrorMessage = "Please enter a username")]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter a password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Please enter a domain")]
        public string Domain { get; set; }
        [Required(ErrorMessage = "Please enter a url")]
        public string URL { get; set; }
        public long selectedFolderIDInhouse { get; set; }
        public string ReturnUrlInhouse { get; set; }
        public string ServerVersionInhouse { get; set; }
        public string SpecificURLIn { get; set; }
        [Required(ErrorMessage = "Please enter an email")]
        public string EmailAddressIn { get; set; }
        public int AccessTypeIn { get; set; }
        public long CreatedCredentialID { get; set; }
        public long CreatedCredentialIDIn { get; set; }
        public List<CCConnection> ExistingConnections { get; set; }
    }
}