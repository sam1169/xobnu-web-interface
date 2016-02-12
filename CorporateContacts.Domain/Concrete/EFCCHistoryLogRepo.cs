using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xobnu.Domain.Concrete
{
    public class EFCCHistoryLogRepo : ICCHistoryLogRepo
    {
        private EFDBContextClient context = new EFDBContextClient();

        public IQueryable<CCHistoryLog> CCHistoryLogs
        {
            get { return context.CCHistoryLog; }
            set { }
        }

        public CCHistoryLog SaveHistoryLog(CCHistoryLog historyLogsObj)
        {

            if (historyLogsObj.ID == 0)
            {
                context.CCHistoryLog.Add(historyLogsObj);
                context.SaveChanges();
            }

            return historyLogsObj;
        }

        public bool DeleteHistoryLogByFieldID(long FieldID)
        {
            List<CCHistoryLog> dbEntryList = context.CCHistoryLog.Where(log => log.FieldID == FieldID).ToList();
            foreach (var dbEntry in dbEntryList)
            {
                if (dbEntry != null)
                {
                    context.CCHistoryLog.Remove(dbEntry);
                    context.SaveChanges();
                }               
            }
            return true;            
        }
    }
}
