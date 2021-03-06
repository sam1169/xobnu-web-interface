﻿using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xobnu.Domain.Concrete
{
    public class EFCCErrorLogRepo : ICCErrorLogRepo
    {
        private EFDbContextErrorLog context = new EFDbContextErrorLog();

        public IQueryable<CCErrorLog> CCErrorLogs
        {
            get { return context.CCErrorLogs; }
            set { }
        }

        public CCErrorLog SaveErrorLog(CCErrorLog errorLogsObj)
        {

            if (errorLogsObj.LogID == 0)
            {
                context.CCErrorLogs.Add(errorLogsObj);
                context.SaveChanges();
            }

            return errorLogsObj;
        }
    }
}
