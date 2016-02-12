using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xobnu.Domain.Entities
{
    public class Notification
    {
        public string Message { get; set; }
        public string RecipientAddress { get; set; }
        public string Subject { get; set; }
    }
}
