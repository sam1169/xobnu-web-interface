using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CorporateContacts.Domain.Entities
{
     [Table("tblTokens")]
    public class CCToken
    {
         public long ID { get; set; }
         public long AccountID { get; set; }
         public string Token { get; set; }
    }
}
