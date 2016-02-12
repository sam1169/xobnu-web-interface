using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CorporateContacts.Domain.Entities
{
    [Table("tblUsers")]
    public class User
    {
        [HiddenInput(DisplayValue = false)]
        public long ID { get; set; }
        [Required(ErrorMessage = "Please enter your name")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [System.Web.Mvc.Compare("Password")]
        [NotMapped]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        
        [NotMapped]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [NotMapped]
        public bool isAdmin { get; set; }

        [HiddenInput(DisplayValue = false)]
        public bool PrimaryUser { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string UserType { get; set; }

        [HiddenInput(DisplayValue = false)]
        public long AccountID { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string GUID { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string Status { get; set; }

        [HiddenInput(DisplayValue = false)]
        public bool EmailVerified { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public bool isPasswordChange { get; set; }
    }
}
