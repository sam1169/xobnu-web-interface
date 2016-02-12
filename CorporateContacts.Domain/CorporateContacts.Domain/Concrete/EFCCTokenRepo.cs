using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Concrete
{
   public class EFCCTokenRepo : ICCTokenRepo 
    {
        private EFDbContextAccounts context = new EFDbContextAccounts();

        public IQueryable<CCToken> CCTokens
        {
            get { return context.CCTokens; }
            set { }
        }

        public CCToken SaveToken(CCToken tokensObj)
        {

            if (tokensObj.ID == 0)
            {
                context.CCTokens.Add(tokensObj);
                context.SaveChanges();
            }

            return tokensObj;
        }

        public bool DeleteToken(long id)
        {
            CCToken dbEntry = context.CCTokens.Find(id);
            if (dbEntry != null)
            {
                context.CCTokens.Remove(dbEntry);
                context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
