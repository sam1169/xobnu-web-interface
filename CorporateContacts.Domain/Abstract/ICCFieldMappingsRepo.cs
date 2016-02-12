using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Abstract
{
    public interface ICCFieldMappingsRepo
    {
        IQueryable<CCFieldMapping> CCFieldsMapping { get; set; }
        CCFieldMapping SaveFieldMapping(CCFieldMapping FieldsMappingObj);     
        void SetConnectionString(string connStr);
        bool DeleteMappingFields(long MappingID);
        bool DeleteMappingFieldBySubscriptionID(long subscriptionID);
        bool SaveAllMappingFields(long id, long cid,string accountGUID);
    }
}
