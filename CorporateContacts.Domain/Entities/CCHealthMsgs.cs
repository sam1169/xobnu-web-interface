using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xobnu.Domain.Entities
{
    [Table("tblCCHealthMsgs")]
    public class CCHealthMsgs
    {
        [Key]
        public long ID { get; set; }
        public string Message { get; set; }
        public DateTime? IssueDateTime { get; set; }
        public DateTime? FixDateTime { get; set; }
        public long HealthLevel { get; set; }
        public Boolean isIssue { get; set; }
    }
}
