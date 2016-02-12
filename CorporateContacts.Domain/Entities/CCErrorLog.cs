using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xobnu.Domain.Entities
{
    [Table("tblCCErrorLogs")]
    public class CCErrorLog
    {
        [Key]
        public long LogID { get; set; }
        public DateTime DateTime { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public long AccountID { get; set; }
        public string AccountGUID { get; set; }
        public long ConnectionID { get; set; }
        public string ErrorType { get; set; }
        public string ErrorMsg { get; set; }
        public string Source { get; set; }
        public string UserEmail { get; set; }
        public string ErrorStackTrace { get; set; }
        public string ErrorMsgUF { get; set; }

        public CCErrorLog()
        {
            ConnectionID = 0;
        }

    }

    
}
