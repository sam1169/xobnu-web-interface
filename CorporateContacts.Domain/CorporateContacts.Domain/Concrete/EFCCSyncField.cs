using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Concrete
{
    public class EFCCSyncField : ICCSyncFieldsRepo
    {
        private EFDBContextClient context = new EFDBContextClient();

        public IQueryable<CCSyncFields> CCSyncFields
        {
            get { return context.CCSyncFields; }
            set { }
        }

        public CCSyncFields SaveSyncFields(CCSyncFields SyncFieldObj)
        {

            if (SyncFieldObj.ID == 0)
            {
                context.CCSyncFields.Add(SyncFieldObj);
                context.SaveChanges();
            }
            else
            {

                CCSyncFields dbEntry = context.CCSyncFields.Find(SyncFieldObj.ID);
                if (dbEntry != null)
                {
                    dbEntry.Name = SyncFieldObj.Name;
                    context.SaveChanges();
                }
            }

            return SyncFieldObj;
        }

        public bool DeleteSyncField(long id)
        {
            CCSyncFields dbEntry = context.CCSyncFields.Find(id);
            if (dbEntry != null)
            {
                context.CCSyncFields.Remove(dbEntry);
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
