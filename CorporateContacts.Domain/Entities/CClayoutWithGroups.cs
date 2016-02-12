using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xobnu.Domain.Entities
{
    public class CClayoutWithGroups
    {
        public List<string> GroupName { get; set; }
        public List<List<CCGroupsWithFieldsAndValues>> LayoutWithValue { get; set; }

    }
}
