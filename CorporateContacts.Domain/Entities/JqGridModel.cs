using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xobnu.Domain.Entities
{
    public class JqGridModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public int TotalPages { get; set; }
        public int Records { get; set; }
        public List<CCViewItems> ItemsTemplatesList { get; set; }
        public List<CCViewCalendarItem> CalenderItemsTemplatesList { get; set; }
        public string IsSessionExpired { get; set; }
    }
}
