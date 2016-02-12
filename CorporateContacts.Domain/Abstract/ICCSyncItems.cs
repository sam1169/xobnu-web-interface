using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Abstract
{
    public interface ICCSyncItems
    {
        IQueryable<CCSyncItems> CCSyncItems { get; set; }
        CCSyncItems SaveSyncItem(CCSyncItems SyncItemObj);
        bool DeleteSyncItem(long SyncItemId);
        void SetConnectionString(string connStr);
    }
}
