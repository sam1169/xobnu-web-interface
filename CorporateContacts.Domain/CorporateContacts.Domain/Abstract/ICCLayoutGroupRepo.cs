using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Abstract
{
    public interface ICCLayoutGroupRepo
    {
        IQueryable<CCLayoutGroup> CCLayoutGroups { get; set; }
        CCLayoutGroup SavelayoutGroup(CCLayoutGroup LayoutGroupObj);
        bool DeletelayoutGroup(long LayoutId);
        void SetConnectionString(string connStr);
        bool DeleteLayoutGroupsByGroupID(long groupid);
        bool DeleteLayoutGroupsByLayoutID(long layoutid);
      
    }
}
