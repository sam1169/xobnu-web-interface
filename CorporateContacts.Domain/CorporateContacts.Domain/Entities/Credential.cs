using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateContacts.Domain.Entities
{
    [Table("tblCredentials")]
    public class Credential
    {
        public long ID { get; set; }
        [Required(ErrorMessage = "Please enter a name")]
        public string Name { get; set; }
        public string URL { get; set; }
        [Required(ErrorMessage = "Please enter an email")]
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter a password")]
        public string Password { get; set; }
        public string ServerVersion { get; set; }
        public bool UseImpersonation { get; set; }
        public bool IsHostedExchange { get; set; }
        public string AccountGUID { get; set; }
        public string Domain { get; set; }
    }

    public class CombinedMinLengthAttribute : ValidationAttribute
    {
        public CombinedMinLengthAttribute(int minLength, params string[] propertyNames)
        {
            this.PropertyNames = propertyNames;
            this.MinLength = minLength;
        }

        public string[] PropertyNames { get; private set; }
        public int MinLength { get; private set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var properties = this.PropertyNames.Select(validationContext.ObjectType.GetProperty);
            var values = properties.Select(p => p.GetValue(validationContext.ObjectInstance, null)).OfType<string>();
            var totalLength = values.Sum(x => x.Length) + Convert.ToString(value).Length;
            if (totalLength < this.MinLength)
            {
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }
            return null;
        }
    }
}
