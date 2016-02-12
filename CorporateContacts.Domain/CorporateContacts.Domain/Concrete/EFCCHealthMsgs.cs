using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Concrete
{
    public class EFCCHealthMsgs : ICCHealthMsgs
    {
        private EFDbContextErrorLog context = new EFDbContextErrorLog();
        

        

        public IQueryable<CCHealthMsgs> CCHealthMsgs
        {
            get { return context.CCHealthMsgs; }
            set { }
        }

        public CCHealthMsgs SaveHealthMsg(CCHealthMsgs healthMsgObj)
        {
            if (healthMsgObj.ID == 0)
            {
                context.CCHealthMsgs.Add(healthMsgObj);
                context.SaveChanges();
            }
            else
            {
                CCHealthMsgs dbEntry = context.CCHealthMsgs.Find(healthMsgObj.ID);
                if (dbEntry != null)
                {
                    dbEntry.Message = healthMsgObj.Message;
                    dbEntry.IssueDateTime = healthMsgObj.IssueDateTime;
                    dbEntry.FixDateTime = healthMsgObj.FixDateTime;
                    dbEntry.HealthLevel = healthMsgObj.HealthLevel;
                    dbEntry.isIssue = healthMsgObj.isIssue;
                    context.SaveChanges();
                }
            }
            return healthMsgObj;
        }
    }
}
