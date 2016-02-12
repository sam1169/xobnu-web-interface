using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Concrete
{
    public class EFCCSyncItems :ICCSyncItems
    {
        private EFDBContextClient context = new EFDBContextClient();

        public IQueryable<CCSyncItems> CCSyncItems
        {
            get { return context.CCSyncItems; }
            set { }
        }

        public CCSyncItems SaveSyncItem(CCSyncItems SyncItemObj)
        {

            if (SyncItemObj.ID == 0)
            {
                context.CCSyncItems.Add(SyncItemObj);
                context.SaveChanges();
            }
            else
            {

                CCSyncItems dbEntry = context.CCSyncItems.Find(SyncItemObj.ID);
                if (dbEntry != null)
                {
                    dbEntry.Tag = SyncItemObj.Tag;
                    context.SaveChanges();
                }
            }

            return SyncItemObj;
        }

        public bool DeleteSyncItem(long id)
        {
            CCSyncItems dbEntry = context.CCSyncItems.Find(id);
            if (dbEntry != null)
            {
                context.CCSyncItems.Remove(dbEntry);
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
