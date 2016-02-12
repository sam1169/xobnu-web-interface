using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Concrete
{
    public class EFCCGroupFieldRepo : ICCGroupFieldRepo 
    {
        private EFDBContextClient context = new EFDBContextClient();

        public IQueryable<CCGroupField> CCGroupsFields
        {
            get { return context.CCGroupFields ; }
            set { }
        }

        public CCGroupField SaveGroupField(CCGroupField groupObj)
        {
            context.CCGroupFields.Add(groupObj);
            context.SaveChanges();
            return groupObj;
        }

        public bool DeleteGroupField(long id)
        {
            CCGroupField dbEntry = context.CCGroupFields.Find(id);
            if (dbEntry != null)
            {
                context.CCGroupFields.Remove(dbEntry);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public void SetConnectionString(string connStr)
        {
            this.context.ConnStr = connStr;
        }

        public bool DeleteFieldsByGroup(long groupid)
        {
            bool result = false;
            var userToDelete = this.context.CCGroupFields 
                              .Where(u => u.GroupID == groupid).ToList();
            if (userToDelete != null)
            {
                foreach (var grpid in userToDelete)
                {
                    result = this.DeleteGroupField(grpid.GrpFieldID);
                }

            }
            return result;
        }


    }
}
