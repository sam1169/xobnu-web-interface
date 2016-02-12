using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;
using Newtonsoft.Json;
using System.Data;

namespace Xobnu.Domain.Concrete
{
    public class EFCCItemRepo : ICCItemRepo
    {
        private EFDBContextClient context = new EFDBContextClient();

        public IQueryable<CCItems> CCContacts
        {
            get { return context.CCContacts; }
            set { }
        }

        public CCItems SaveContact(CCItems contactObj)
        {
            if (contactObj.ItemID == 0)
            {
                context.CCContacts.Add(contactObj);
                context.SaveChanges();
            }
            else
            {
                CCItems dbEntry = context.CCContacts.Find(contactObj.ItemID);
                if (dbEntry != null)
                {
                    dbEntry.LastUpdated = DateTime.Now.ToUniversalTime();
                    context.SaveChanges();
                }

            }

            return contactObj;
        }

        public bool UpdateContact(CCItems contactObj)
        {
            CCItems dbEntry = context.CCContacts.Find(contactObj.ItemID);
            if (dbEntry != null)
            {
                dbEntry.LastUpdated = DateTime.Now.ToUniversalTime();
                dbEntry.DeDupeValue = contactObj.DeDupeValue;

                if (dbEntry.TextBody != contactObj.TextBody)
                    dbEntry.TextBody = contactObj.TextBody;

                if (dbEntry.Notes != contactObj.Notes)
                    dbEntry.Notes = contactObj.Notes;

                if (dbEntry.isDeleted != contactObj.isDeleted)
                {
                    CCHistoryLog HistoryLog = new CCHistoryLog();
                    HistoryLog.AccountGUID = dbEntry.AccountGUID;
                    HistoryLog.Date = DateTime.Now;
                    HistoryLog.FieldID = 0;
                    HistoryLog.ItemID = dbEntry.ItemID;
                    HistoryLog.NewValue = "";
                    HistoryLog.OldValue = "";
                    HistoryLog.Source = "Web";
                    HistoryLog.Action = "Delete-Revert";
                    HistoryLog.ConnectionID = 0;
                    context.CCHistoryLog.Add(HistoryLog);
                    context.SaveChanges();

                    dbEntry.isDeleted = contactObj.isDeleted;
                }
                context.SaveChanges();
                return true;
            }

            return false;
        }

        public bool DeleteContact(long id)
        {
            CCItems dbEntry = context.CCContacts.Find(id);
            if (dbEntry != null)
            {
                //  context.CCContacts.Remove(dbEntry);
                dbEntry.LastUpdated = DateTime.Now.ToUniversalTime();
                dbEntry.isDeleted = true;
                context.SaveChanges();

                CCHistoryLog HistoryLog = new CCHistoryLog();
                HistoryLog.AccountGUID = dbEntry.AccountGUID;
                HistoryLog.Date = DateTime.Now;
                HistoryLog.FieldID = 0;
                HistoryLog.ItemID = dbEntry.ItemID;
                HistoryLog.NewValue = "" ;
                HistoryLog.OldValue = "";
                HistoryLog.Source = "Web";
                HistoryLog.Action = "Delete";
                HistoryLog.ConnectionID = 0;
                context.CCHistoryLog.Add(HistoryLog);
                context.SaveChanges();

                return true;
            }
            return false;
        }

        public void SetConnectionString(string connStr)
        {
            this.context.ConnStr = connStr;
        }

        public List<CCViewCalendarItem> GetItemsCalenderLists(int pageNumber, int recordsPerPage, string sortColumn, string sortOrder, string sortBy, long folderID, bool search, string fieldName, string fieldValue, string timeZone, string accountGUID)
        {
            List<string> fieldsList = null;
            fieldsList = new List<string> { "Start", "End", "Location", "Subject", "LawyerName", "CaseManager" };

            List<CCViewCalendarItem> viewItems = new List<CCViewCalendarItem>();

            List<CCFolderField> selectedcontactsFields = null;

            var ContactsFields = context.CCFolderFields
                                   .Where(u => u.FolderID == folderID).ToList();

            var ContactsFieldss = context.CCFolderFields
                                   .Where(u => u.FolderID == folderID).ToList();

            foreach (var contactfield in ContactsFields)
            {
                var res = fieldsList.FirstOrDefault(fname => fname.ToString() == contactfield.FieldName);
                if (res == null)
                {
                    // remove  field
                    ContactsFieldss.RemoveAll(exist => exist.FieldID == contactfield.FieldID);
                }

            }

            selectedcontactsFields = ContactsFieldss;

            long fieldID = 0;
            if (sortColumn != "") { fieldID = selectedcontactsFields.FirstOrDefault(id => id.FieldName == sortColumn).FieldID; }

            long searchFieldID = 0;
            if (fieldName != "" && fieldName != "undefined") { searchFieldID = selectedcontactsFields.FirstOrDefault(id => id.FieldName == fieldName).FieldID; }

            var query = (from co in this.context.CCContacts
                         join fv in this.context.CCFieldValues on co.ItemID equals fv.ItemID into gj
                         where co.FolderID == folderID && co.isDeleted == false && co.AccountGUID == accountGUID
                         select new { val = gj });

            if (search)
            {
                query = query
                              .Where(e => e.val.Select(v => v).Where(it => it.FieldID == searchFieldID).FirstOrDefault().Value == fieldValue)
                              .OrderByDescending(e => e.val.Select(v => v).Where(it => it.FieldID == searchFieldID).FirstOrDefault().Value)
                              .Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);
            }
            else if (sortColumn == "")
            {
                query = query
                             .OrderBy(e => e.val.Select(v => v).Where(it => it.FieldID == fieldID).FirstOrDefault().Value)
                             .Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);
            }
            else if (sortOrder == "asc")
            {
                query = query
                             .OrderBy(e => e.val.Select(v => v).Where(it => it.FieldID == fieldID).FirstOrDefault().Value)
                             .Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);
            }
            else if (sortOrder == "desc")
            {
                query = query
                               .OrderByDescending(e => e.val.Select(v => v).Where(it => it.FieldID == fieldID).FirstOrDefault().Value)
                               .Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);
            }


            //var cou = query.Count();

            foreach (var item in query)
            {
                CCViewCalendarItem viewitem = new CCViewCalendarItem();
                foreach (var conf in selectedcontactsFields)
                {
                    var res = item.val.Where(fid => fid.FieldID == conf.FieldID).FirstOrDefault();

                    if (res != null)
                    {
                        if (conf.FieldName == "Start")
                        {
                            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                            TimeSpan offset = tzi.GetUtcOffset(DateTime.Now);
                            DateTime convertedDate = DateTime.Parse(res.Value) + offset;
                            string format = "yyyy-MM-dd HH:mm";
                            viewitem.Start = convertedDate.ToString(format);

                        }
                        else if (conf.FieldName == "End")
                        {
                            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                            TimeSpan offset = tzi.GetUtcOffset(DateTime.Now);
                            DateTime convertedDate = DateTime.Parse(res.Value) + offset;
                            string format = "yyyy-MM-dd HH:mm";
                            viewitem.End = convertedDate.ToString(format);                          
                        }
                        else if (conf.FieldName == "Location") { viewitem.Location = res.Value; }
                        else if (conf.FieldName == "Subject") { viewitem.Subject = res.Value; }
                        else if (conf.FieldName == "LawyerName") { viewitem.LawyerName = res.Value; }
                        else if (conf.FieldName == "CaseManager") { viewitem.CaseManager = res.Value; }
                        viewitem.ID = res.ItemID;

                    }
                    else
                    {
                        if (conf.FieldName == "Start") { viewitem.Start = null; }
                        else if (conf.FieldName == "End") { viewitem.End = null; }
                        else if (conf.FieldName == "Location") { viewitem.Location = null; }
                        else if (conf.FieldName == "Subject") { viewitem.Subject = null; }
                        else if (conf.FieldName == "LawyerName") { viewitem.LawyerName = null; }
                        else if (conf.FieldName == "CaseManager") { viewitem.CaseManager = null; }
                    }
                }
                viewItems.Add(viewitem);
            }

            return viewItems;

        }

        public List<CCViewItems> GetItemsLists(int pageNumber, int recordsPerPage, string sortColumn, string sortOrder, string sortBy, long folderID, bool search, string fieldName, string fieldValue, string accountGUID)
        {
            List<string> fieldsList = null;
            fieldsList = new List<string> { "GivenName", "LastName", "JobTitle", "CompanyName", "MobilePhone", "BusinessPhone", "EmailAddress1" };

            List<CCViewItems> viewItems = new List<CCViewItems>();

            List<CCFolderField> selectedcontactsFields = null;

            var ContactsFields = context.CCFolderFields
                                   .Where(u => u.FolderID == folderID).ToList();

            var ContactsFieldss = context.CCFolderFields
                                   .Where(u => u.FolderID == folderID).ToList();

            foreach (var contactfield in ContactsFields)
            {
                var res = fieldsList.FirstOrDefault(fname => fname.ToString() == contactfield.FieldName);
                if (res == null)
                {
                    // remove  field
                    ContactsFieldss.RemoveAll(exist => exist.FieldID == contactfield.FieldID);
                }

            }

            selectedcontactsFields = ContactsFieldss;
            if (sortColumn == "")
            {
                sortColumn = "GivenName";
            }

            long fieldID = 0;
            if (sortColumn != "") { fieldID = selectedcontactsFields.FirstOrDefault(id => id.FieldName == sortColumn).FieldID; }
            

            long searchFieldID = 0;
            if (fieldName != "" && fieldName != "undefined") { searchFieldID = selectedcontactsFields.FirstOrDefault(id => id.FieldName == fieldName).FieldID; }

            var query = (from co in this.context.CCContacts
                         join fv in this.context.CCFieldValues on co.ItemID equals fv.ItemID into gj
                         where co.FolderID == folderID && co.isDeleted == false && co.AccountGUID == accountGUID
                         select new { val = gj });
            if (search)
            {
                query = query
                              .Where(e => e.val.Select(v => v).Where(it => it.FieldID == searchFieldID).FirstOrDefault().Value == fieldValue)
                              .OrderByDescending(e => e.val.Select(v => v).Where(it => it.FieldID == searchFieldID).FirstOrDefault().Value)
                              .Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);
            }
            else if (sortColumn == "")
            {
                query = query
                             .OrderBy(e => e.val.Select(v => v).Where(it => it.FieldID == fieldID).FirstOrDefault().Value)
                             .Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);

            }
            else if (sortOrder == "asc")
            {
                query = query
                             .OrderBy(e => e.val.Select(v => v).Where(it => it.FieldID == fieldID).FirstOrDefault().Value)
                             .Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);
            }
            else if (sortOrder == "desc")
            {
                query = query
                               .OrderByDescending(e => e.val.Select(v => v).Where(it => it.FieldID == fieldID).FirstOrDefault().Value)
                               .Skip((pageNumber - 1) * recordsPerPage).Take(recordsPerPage);
            }

            var allcontItems = this.context.CCContacts.ToList();
            //var cou = query.Count();
            foreach (var item in query)
            {
                CCViewItems viewitem = new CCViewItems();
                foreach (var conf in selectedcontactsFields)
                {
                    var res = item.val.Where(fid => fid.FieldID == conf.FieldID).FirstOrDefault();

                    if (res != null)
                    {
                        if (conf.FieldName == "GivenName") { viewitem.GivenName = res.Value; }
                        else if (conf.FieldName == "LastName") { viewitem.LastName = res.Value; }
                        else if (conf.FieldName == "JobTitle") { viewitem.JobTitle = res.Value; }
                        else if (conf.FieldName == "CompanyName") { viewitem.CompanyName = res.Value; }
                        else if (conf.FieldName == "MobilePhone") { viewitem.MobilePhone = res.Value; }
                        else if (conf.FieldName == "BusinessPhone") { viewitem.BusinessPhone = res.Value; }
                        else if (conf.FieldName == "EmailAddress1") { viewitem.EmailAddress1 = res.Value; }
                        viewitem.ID = res.ItemID;

                    }
                    else
                    {
                        if (conf.FieldName == "GivenName") { viewitem.GivenName = null; }
                        else if (conf.FieldName == "LastName") { viewitem.LastName = null; }
                        else if (conf.FieldName == "JobTitle") { viewitem.JobTitle = null; }
                        else if (conf.FieldName == "CompanyName") { viewitem.CompanyName = null; }
                        else if (conf.FieldName == "MobilePhone") { viewitem.MobilePhone = null; }
                        else if (conf.FieldName == "BusinessPhone") { viewitem.BusinessPhone = null; }
                        else if (conf.FieldName == "EmailAddress1") { viewitem.EmailAddress1 = null; }
                    }
                }

                // get item for DistList
                var itemID = item.val.FirstOrDefault().ItemID;
                CCItems itemInfor = allcontItems.Find(id => id.ItemID == itemID);               
                if (itemInfor != null)
                {
                    if (itemInfor.isDistGroup)
                    {
                        viewitem.GivenName = itemInfor.DeDupeValue;
                        viewitem.LastName = "Contact List";
                        viewitem.ID = itemID;
                    }
                }

                viewItems.Add(viewitem);
            }

            return viewItems;

        }

        public JqGridModel GetJQModel(int pageNumber, int recordsPerPage, string sortColumn, string sortOrder, string sortBy, long folderID, int type, string searchField, string searchValue, string timeZone,string accountGUID)
        {
            bool search = false;
            if (searchField != "undefined" && searchValue != "undefined") { search = true; }

            var jqGridObject = new JqGridModel();

            var query = (from co in this.context.CCContacts
                         join fv in this.context.CCFieldValues on co.ItemID equals fv.ItemID into gj
                         where co.FolderID == folderID && co.isDeleted == false && co.AccountGUID==accountGUID
                         select new { conid = co.ItemID, val = gj });
            if (type == 1)
            {
                jqGridObject = new JqGridModel
                {
                    PageIndex = pageNumber,
                    PageSize = recordsPerPage,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    ItemsTemplatesList = GetItemsLists(pageNumber, recordsPerPage, sortColumn, sortOrder, sortBy, folderID, search, searchField, searchValue, accountGUID),
                    IsSessionExpired = "false"
                };
            }
            else
            {
                jqGridObject = new JqGridModel
                {
                    PageIndex = pageNumber,
                    PageSize = recordsPerPage,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    CalenderItemsTemplatesList = GetItemsCalenderLists(pageNumber, recordsPerPage, sortColumn, sortOrder, sortBy, folderID, search, searchField, searchValue, timeZone, accountGUID),
                    IsSessionExpired = "false"
                };

            }

            var totalCount = query.Count();
            jqGridObject.Records = totalCount;
            if ((totalCount % jqGridObject.PageSize) == 0)
            {
                jqGridObject.TotalPages = (totalCount / jqGridObject.PageSize);
            }
            else
            {
                var totalPages = (totalCount / jqGridObject.PageSize);
                jqGridObject.TotalPages = totalPages + 1;
            }

            return jqGridObject;
        }

        public List<CCContactWithFields> GetContacts(long ID, int pageSize, int pageNumber, List<CCFolderField> selectedcontactsFields)
        {
            List<CCContactWithFields> contactList = new List<CCContactWithFields>();

            var query = (from co in this.context.CCContacts
                         join fv in this.context.CCFieldValues on co.ItemID equals fv.ItemID into gj
                         where co.FolderID == ID && co.isDeleted == false
                         select new { conid = co.ItemID, val = gj })
                        .OrderBy(c => c.conid)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize);


            var nof = query.Count();


            foreach (var item in query)
            {
                CCContactWithFields contact = new CCContactWithFields();
                List<string> fieldValueList = new List<string>();

                foreach (var conf in selectedcontactsFields)
                {
                    var res = item.val.Where(fid => fid.FieldID == conf.FieldID).FirstOrDefault();

                    if (res != null)
                    {
                        if (conf.FieldCaption == "Start Time")
                        {
                            string format = "yyyy-MM-dd HH:mm";
                            DateTime convertedDate = DateTime.Parse(res.Value);
                            DateTime localDate = convertedDate.ToLocalTime();
                            fieldValueList.Add(localDate.ToString(format));
                        }
                        else { fieldValueList.Add(res.Value); }

                    }
                    else
                    {
                        fieldValueList.Add(null);
                    }

                }
                contact.ContactID = item.conid;
                contact.FieldValues = fieldValueList;
                contactList.Add(contact);

            }

            return contactList;

        }

        public long CreateContact(long fid, string augid)
        {
            CCItems objcontact = new CCItems();
            objcontact.FolderID = fid;
            objcontact.LastUpdated = DateTime.Now.ToUniversalTime();
            objcontact.Created = DateTime.Now.ToUniversalTime();
            objcontact.isDistGroup = false;
            objcontact.isRecurring = false;
            objcontact.AccountGUID = augid;
            var cont = this.SaveContact(objcontact);

            return cont.ItemID;

        }

        public void UpdateContactTable()
        {
            //CCItems dbEntry = new CCItems();// = context.CCContacts.FirstOrDefault();
            //context.Entry(dbEntry).State = System.Data.Entity.EntityState.Modified;
            //context.SaveChanges();
        }

        public void DeleteItems(long folderID)
        {
            List<CCItems> items = context.CCContacts.Where(i => i.FolderID == folderID).ToList();

            foreach (var item in items)
            {
                CCItems dbEntry = context.CCContacts.Find(item.ItemID);
                if (dbEntry != null)
                {
                    // remove field value for item  
                    var fieldvalues = context.CCFieldValues.Where(fv => fv.ItemID == item.ItemID).ToList();
                    foreach (var fieldvalue in fieldvalues)
                    {
                        CCFieldValue dbfv = context.CCFieldValues.Find(fieldvalue.ValueID);
                        if (dbfv != null)
                        {
                            context.CCFieldValues.Remove(dbfv);
                        }
                    }
                    // remove item 
                    context.CCContacts.Remove(dbEntry);
                }
            }
            context.SaveChanges();
        }

    }
}
