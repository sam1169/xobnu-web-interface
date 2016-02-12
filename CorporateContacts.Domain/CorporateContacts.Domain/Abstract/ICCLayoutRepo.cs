using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Abstract
{
    public interface ICCLayoutRepo
    {
        IQueryable<CCLayout> CCLayouts { get; set; }
        CCLayout Savelayout(CCLayout LayoutObj);
        bool Deletelayout(long LayoutId);
        void SetConnectionString(string connStr);
    }
}
