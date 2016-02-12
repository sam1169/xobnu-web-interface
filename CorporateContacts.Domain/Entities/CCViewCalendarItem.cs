using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xobnu.Domain.Entities
{
    public class CCViewCalendarItem
    {
        public string Start { get; set; }
        public string Duration { get; set; }
        public string End { get; set; }       
        public string Location { get; set; }
        public string Subject { get; set; }
        public long ID { get; set; }
        public string LawyerName { get; set; }
        public string CaseManager { get; set; }
    }
}
