using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Abstract
{
    public interface ICCSyncFieldsRepo
    {
        IQueryable<CCSyncFields> CCSyncFields { get; set; }
        CCSyncFields SaveSyncFields(CCSyncFields SyncFieldObj);
        bool DeleteSyncField(long SyncFieldId);
        void SetConnectionString(string connStr);
    }
}
