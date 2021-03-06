﻿using CorporateContacts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateContacts.Domain.Abstract
{
    public interface ICCHistoryLogRepo
    {
        IQueryable<CCHistoryLog> CCHistoryLogs { get; set; }
        CCHistoryLog SaveHistoryLog(CCHistoryLog historyLogsObj);
        bool DeleteHistoryLogByFieldID(long FieldID);
    }
}
