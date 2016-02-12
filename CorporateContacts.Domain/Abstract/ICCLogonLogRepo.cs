using Xobnu.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xobnu.Domain.Abstract
{
    public interface ICCLogonLogRepo
    {
        IQueryable<CCLogonLog> CCLogonLogs { get; set; }
        CCLogonLog SaveLogonLog(CCLogonLog logonLogsObj);
    }
}
