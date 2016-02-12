using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xobnu.Domain.Entities
{
   public    class CCContactWithFields
    {
        public long ContactID { get; set; }
        public List<string> FieldValues{ get; set; }
    }
}
