using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateContacts.Domain.Concrete
{
    public class EFCCLogonLogRepo : ICCLogonLogRepo
    {
        private EFDbContextLogonLog context = new EFDbContextLogonLog();

        public IQueryable<CCLogonLog> CCLogonLogs
        {
            get { return context.CCLogonLogs; }
            set { }
        }

        public CCLogonLog SaveLogonLog(CCLogonLog logonLogsObj)
        {
            if (logonLogsObj.LogID == 0)
            {
                context.CCLogonLogs.Add(logonLogsObj);
                context.SaveChanges();
            }

            return logonLogsObj;
        }
    }
}
