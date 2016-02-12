using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Concrete
{
    public class EFCCFieldValueRepo : ICCFieldValuesRepo
    {
        private EFDBContextClient context = new EFDBContextClient();
        private ICCHistoryLogRepo CCHistoryLogRepository;

        public EFCCFieldValueRepo(ICCHistoryLogRepo historyLogRepository)
        {
            CCHistoryLogRepository = historyLogRepository;
        }

        public IQueryable<CCFieldValue> CCFieldValues
        {
            get { return context.CCFieldValues; }
            set { }
        }

        public CCFieldValue RevertChangeToFieldValues(CCFieldValue FieldValueObj, CCHistoryLog HistLogObj)
        {
            CCFieldValue dbEntry = context.CCFieldValues.Find(FieldValueObj.ValueID);
            if (dbEntry != null)
            {
                CCHistoryLog HistoryLog = new CCHistoryLog();
                HistoryLog.AccountGUID = FieldValueObj.AccountGUID;
                HistoryLog.Date = DateTime.Now;
                HistoryLog.FieldID = dbEntry.FieldID;
                HistoryLog.ItemID = dbEntry.ItemID;
                HistoryLog.NewValue = FieldValueObj.Value;
                HistoryLog.OldValue = HistLogObj.NewValue;
                HistoryLog.Source = "Web";
                HistoryLog.Action = "Update";
                HistoryLog.ConnectionID = 0;
                context.CCHistoryLog.Add(HistoryLog);

                dbEntry.Value = FieldValueObj.Value;    

                context.SaveChanges(); 
            }
            return FieldValueObj;
        }

        public CCFieldValue SaveFieldValues(CCFieldValue FieldValueObj)
        {
            if (FieldValueObj.ValueID == 0)
            {    
                context.CCFieldValues.Add(FieldValueObj);
                //context.SaveChanges();

                CCHistoryLog HistoryLog = new CCHistoryLog();
                HistoryLog.AccountGUID = FieldValueObj.AccountGUID;
                HistoryLog.Date = DateTime.Now;
                HistoryLog.FieldID = FieldValueObj.FieldID;
                HistoryLog.ItemID = FieldValueObj.ItemID;
                HistoryLog.NewValue = FieldValueObj.Value;
                HistoryLog.OldValue = "";
                HistoryLog.Source = "Web";
                HistoryLog.Action = "Insert";
                HistoryLog.ConnectionID = 0;
                context.CCHistoryLog.Add(HistoryLog);
                context.SaveChanges();   
            }
            else
            {
                CCFieldValue dbEntry = context.CCFieldValues.Find(FieldValueObj.ValueID);
                if (dbEntry != null)
                {
                    if (dbEntry.Value != FieldValueObj.Value)
                    {
                        CCHistoryLog HistoryLog = new CCHistoryLog();
                        HistoryLog.AccountGUID = FieldValueObj.AccountGUID;
                        HistoryLog.Date = DateTime.Now;
                        HistoryLog.FieldID = dbEntry.FieldID;
                        HistoryLog.ItemID = dbEntry.ItemID;
                        HistoryLog.NewValue = FieldValueObj.Value;
                        HistoryLog.OldValue = dbEntry.Value;
                        HistoryLog.Source = "Web";
                        HistoryLog.Action = "Update";
                        HistoryLog.ConnectionID = 0;
                        context.CCHistoryLog.Add(HistoryLog);
                        //context.SaveChanges();

                        dbEntry.LastUpdated = DateTime.UtcNow;
                    }
                    dbEntry.Value = FieldValueObj.Value;    
                    context.SaveChanges();
                }
            
            }
            return FieldValueObj;
        }

        public List<CCFieldValue> SaveFieldsObjValues(List<CCFieldValue> FieldValues)
        {
            foreach (CCFieldValue fieldvalues in FieldValues)
            {
                context.CCFieldValues.Add(fieldvalues);

                CCHistoryLog HistoryLog = new CCHistoryLog();
                HistoryLog.AccountGUID = fieldvalues.AccountGUID;
                HistoryLog.Date = DateTime.Now;
                HistoryLog.FieldID = fieldvalues.FieldID;
                HistoryLog.ItemID = fieldvalues.ItemID;
                HistoryLog.NewValue = fieldvalues.Value;
                HistoryLog.OldValue = "";
                HistoryLog.Source = "Web";
                HistoryLog.Action = "Insert";
                HistoryLog.ConnectionID = 0;
                context.CCHistoryLog.Add(HistoryLog);
                //context.SaveChanges();   
            }
            context.SaveChanges();
            return FieldValues;
        }

        public bool DeleteFieldValue(long id)
        {
            CCFieldValue  dbEntry = context.CCFieldValues.Find(id);
            if (dbEntry != null)
            {
                CCHistoryLog HistoryLog = new CCHistoryLog();
                HistoryLog.AccountGUID = dbEntry.AccountGUID;
                HistoryLog.Date = DateTime.Now;
                HistoryLog.FieldID = dbEntry.FieldID;
                HistoryLog.ItemID = dbEntry.ItemID;
                HistoryLog.NewValue = dbEntry.Value;
                HistoryLog.OldValue = "";
                HistoryLog.Source = "Web";
                HistoryLog.Action = "Delete";
                context.CCHistoryLog.Add(HistoryLog);

                context.CCFieldValues.Remove(dbEntry);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DeleteFieldValueByITemID(long id)
        {
            List<CCFieldValue> dbEntryList = context.CCFieldValues.Where(fv => fv.ItemID==id).ToList();

            foreach (var dbEntry in dbEntryList)
            {
                if (dbEntry != null)
                {
                    CCHistoryLog HistoryLog = new CCHistoryLog();
                    HistoryLog.AccountGUID = dbEntry.AccountGUID;
                    HistoryLog.Date = DateTime.Now;
                    HistoryLog.FieldID = dbEntry.FieldID;
                    HistoryLog.ItemID = dbEntry.ItemID;
                    HistoryLog.NewValue = dbEntry.Value;
                    HistoryLog.OldValue = "";
                    HistoryLog.Source = "Web";
                    HistoryLog.Action = "Delete";
                    context.CCHistoryLog.Add(HistoryLog);

                    context.CCFieldValues.Remove(dbEntry);
                    context.SaveChanges();
                    
                }            
            }
            return true;
        }

        public void SetConnectionString(string connStr)
        {
            this.context.ConnStr = connStr;
        }
    }
}
