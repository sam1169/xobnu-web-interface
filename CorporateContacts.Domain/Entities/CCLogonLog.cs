using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xobnu.Domain.Entities
{
    [Table("tblCCLogonLogs")]
    public class CCLogonLog
    {
        [Key]
        public long LogID { get; set; }
        public DateTime DateTime { get; set; }
    }
}
