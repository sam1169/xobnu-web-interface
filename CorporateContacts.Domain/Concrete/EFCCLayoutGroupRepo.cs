using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Concrete
{
   public class EFCCLayoutGroupRepo : ICCLayoutGroupRepo 
    {
        private EFDBContextClient context = new EFDBContextClient();

        public IQueryable<CCLayoutGroup> CCLayoutGroups
        {
            get { return context.CCLayoutGroups; }
            set { }
        }

        public CCLayoutGroup SavelayoutGroup(CCLayoutGroup layoutgroupObj)
        {
            context.CCLayoutGroups.Add(layoutgroupObj);
            context.SaveChanges();
            return layoutgroupObj;
        }

        public bool DeletelayoutGroup(long id)
        {
            CCLayoutGroup  dbEntry = context.CCLayoutGroups.Find(id);
            if (dbEntry != null)
            {
                context.CCLayoutGroups.Remove(dbEntry);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public void SetConnectionString(string connStr)
        {
            this.context.ConnStr = connStr;
        }

       public bool DeleteLayoutGroupsByGroupID(long groupid)
        {
            bool res = false;
            var layoutgroupfield = this.context.CCLayoutGroups
                .Where(f => f.GroupID == groupid).ToList();

            if (layoutgroupfield != null)
            {
                foreach (var gid in layoutgroupfield)
                {
                    res = this.DeletelayoutGroup(gid.LayoutGrpID);
                }
            }
            return true;
        }

       public bool DeleteLayoutGroupsByLayoutID(long layoutid)
       {
           var layoutgroup = this.context.CCLayoutGroups
                               .Where(u => u.LayoutID == layoutid).ToList();
           if (layoutgroup != null)
           {
               foreach (var layid in layoutgroup)
               {
                   bool result = this.DeletelayoutGroup(layid.LayoutGrpID);
               }

           }

           return true;
       }
       

    }
}
