using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Xobnu.Domain.Entities
{
    [Table("tblAccounts")]
    public class Account
    {
        [HiddenInput(DisplayValue = false)]
        public long ID { get; set; }

        [Display(Name="A name for your account")]
        [Required(ErrorMessage = "Please enter your company name")]
        public string AccountName { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string TablePrefix { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int AdditionalDiscount { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int PlanID { get; set; }
        [HiddenInput(DisplayValue = false)]
       // public string ConnectionString { get; set; }
        public string StripeCustomerID { get; set; }
        public string TimeZone { get; set; }
        public string BusinessAddress { get; set; }
        public string AccountGUID { get; set; }
        public string Telephone { get; set; }
        public DateTime? TrialEnds { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Boolean HasPurchased { get; set; }
        public Boolean? isOverFlow { get; set; }
        public Boolean? isPaymentIssue { get; set; }
        public short SyncPeriod { get; set; }

    }
}
