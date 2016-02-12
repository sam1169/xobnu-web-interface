using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Abstract
{
   public interface ICCFieldValuesRepo
    {
        IQueryable<CCFieldValue > CCFieldValues { get; set; }
        CCFieldValue  SaveFieldValues(CCFieldValue  FieldValueObj);
        CCFieldValue RevertChangeToFieldValues(CCFieldValue FieldValueObj, CCHistoryLog HistLogObj);
        List<CCFieldValue> SaveFieldsObjValues(List<CCFieldValue> FieldValuesObj);
        bool DeleteFieldValue(long ContactId);
        bool DeleteFieldValueByITemID(long ItemID);
        void SetConnectionString(string connStr);
    }
}
