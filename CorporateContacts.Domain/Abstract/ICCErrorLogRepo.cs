using Xobnu.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xobnu.Domain.Abstract
{
    public interface ICCErrorLogRepo
    {
        IQueryable<CCErrorLog> CCErrorLogs { get; set; }
        CCErrorLog SaveErrorLog(CCErrorLog errorLogsObj);
    }
}
