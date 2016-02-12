using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Concrete
{
    public class EFLayoutRepo : ICCLayoutRepo
    {
        private EFDBContextClient context = new EFDBContextClient();

        public IQueryable<CCLayout> CCLayouts
        {
            get { return context.CCLayouts; }
            set { }
        }

        public CCLayout Savelayout(CCLayout layoutObj)
        {
            context.CCLayouts.Add(layoutObj);
            context.SaveChanges();
            return layoutObj;
        }

        public bool Deletelayout(long id)
        {
            CCLayout dbEntry = context.CCLayouts.Find(id);
            if (dbEntry != null)
            {
                context.CCLayouts.Remove(dbEntry);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public void SetConnectionString(string connStr)
        {
            this.context.ConnStr = connStr;
        }
    }
}
