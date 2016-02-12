using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;
using System.Data;
using Xobnu.WebUI.Models;

namespace Xobnu.WebUI.Util
{
    public class ItemsImporter
    {
        ICCFolderFieldRepo CCFieldRepository;
        ICCFieldValuesRepo CCFieldValueRepository;
        DataInputHelper inputdatas;
        ICCItemRepo CCItemRepository;
        bool IsFieldValueMatch = true;
        int NumberOfContacts = 0;
        List<CCFieldValue> ObjFieldValues = new List<CCFieldValue>();



        public ItemsImporter(ICCFolderFieldRepo field, ICCFieldValuesRepo fieldvalue, ICCItemRepo Item)
        {
            CCFieldRepository = field;
            CCFieldValueRepository = fieldvalue;
            CCItemRepository = Item;

        }

        public long ImportInputContacts(long fid, string aguid, string path, string fileExtension, long maxItemImportCount)
        {
            if (inputdatas == null)
                inputdatas = new DataInputHelper(path, fileExtension);

            List<CCFolderField> FieldsByFolderID = null;
            FieldsByFolderID = CCFieldRepository.CCFolderFields.Where(id => id.FolderID == fid).ToList();
            DataTable valus = inputdatas.GetImportExcel();
            bool _readheader = true;
            IsFieldValueMatch = false;
            var _availablevalue = new List<Tuple<long, bool>>();

            var totalRowCount = (valus.Rows.Count)-1;

            if (totalRowCount <= maxItemImportCount)
            {
                foreach (DataRow row in valus.Rows)
                {
                    AddDedupeViewModel dedupe = new AddDedupeViewModel();
                    int _fieldcount = 0;
                    long contectID = 0;
                    string notes = "";
                    if (!_readheader && IsFieldValueMatch)
                    {
                        contectID = CCItemRepository.CreateContact(fid, aguid);
                    }
                    foreach (DataColumn col in valus.Columns)
                    {
                        string _colname = row[col].ToString();
                        if (_readheader) // run when it header 
                        {
                            // set correct Header to Field name
                            var reponse = FieldsByFolderID.Find(name => name.FieldName == _colname);
                            if (reponse != null)
                            {
                                _availablevalue.Add(new Tuple<long, bool>(reponse.FieldID, true));
                                IsFieldValueMatch = true;
                            }
                            else
                            {
                                _availablevalue.Add(new Tuple<long, bool>(0, false));
                            }

                        }
                        else
                        { // reu when it columns value
                            if (_availablevalue[_fieldcount].Item2)
                            {
                                CCFieldValue objFieldValue = new CCFieldValue();
                                objFieldValue.FieldID = _availablevalue[_fieldcount].Item1;
                                if (_colname == String.Empty) objFieldValue.Value = String.Empty;
                                else objFieldValue.Value = _colname;
                                objFieldValue.ItemID = contectID;
                                objFieldValue.LastUpdated = DateTime.Now.ToUniversalTime();
                                objFieldValue.AccountGUID = aguid;
                                ObjFieldValues.Add(objFieldValue);

                                // update dedupe                          
                                var fieldName = FieldsByFolderID.Find(id => id.FieldID == _availablevalue[_fieldcount].Item1).FieldCaption;
                                if (fieldName == "First Name") { dedupe.FirstName = _colname; }
                                if (fieldName == "Middle Name") { dedupe.MiddleName = _colname; }
                                if (fieldName == "Last Name") { dedupe.LastName = _colname; }
                                if (fieldName == "Company") { dedupe.CompanyName = _colname; }
                                if (fieldName == "Email Address") { dedupe.Email = _colname; }
                                // End update dedupe

                                if(fieldName == "Notes")
                                    notes = _colname; 

                            }

                            _fieldcount++;

                            
                        }
                    }
                    bool res = UpdateContact(dedupe, contectID, 1, notes);
                    _readheader = false;

                }

                var savedfields = CCFieldValueRepository.SaveFieldsObjValues(ObjFieldValues);

                if (IsFieldValueMatch == false) NumberOfContacts = 0;
                else NumberOfContacts = valus.Rows.Count;




                return NumberOfContacts;
            }
            else
            {
                return -2;
            }

            
        }

        public bool ImportSingleContact(AddContactViewModel objContact, int type,string accountGUID, string timeZone)
        {
            List<string> fieldValus = objContact.FolderValues.Split('|').ToList();
            List<CCFieldValue> ObjFieldValues = new List<CCFieldValue>();
            List<CCFieldValue> savedfields = new List<CCFieldValue>();
            AddDedupeViewModel dedupe = new AddDedupeViewModel();
            DateTime startTime;
            DateTime endTime;
            string notes = "";

            long contectID = CCItemRepository.CreateContact(objContact.FolderID, accountGUID);
            if (contectID > 0)
            {
                List<CCFolderField> folderFields = new List<CCFolderField>();
                folderFields = CCFieldRepository.CCFolderFields.Where(fid => fid.FolderID == objContact.FolderID & fid.AccountGUID==accountGUID).ToList();

                int i = 0;
                foreach (var field in fieldValus)
                {
                    if (field != "")
                    {
                        var fieldName = folderFields[i].FieldCaption;
                        if (type == 1)
                        {
                            if (fieldName == "First Name") { dedupe.FirstName = field; }
                            if (fieldName == "Middle Name") { dedupe.MiddleName = field; }
                            if (fieldName == "Last Name") { dedupe.LastName = field; }
                            if (fieldName == "Company") { dedupe.CompanyName = field; }
                            if (fieldName == "Email Address") { dedupe.Email = field; }
                        }
                        else
                        {
                            if (fieldName == "Subject") { dedupe.Subject = field; }
                            if (fieldName == "Start Time")
                            {
                                string format = "yyyy-MM-dd HH:mm";
                                startTime = DateTime.Parse(field);
                                dedupe.StartDateTime = ConvertLocaltoUTC(startTime, timeZone).ToString(format);
                            }
                            if (fieldName == "End Time")
                            {
                                string format = "yyyy-MM-dd HH:mm";
                                //endTime = DateTimeOffset.Parse(field).UtcDateTime;
                                //dedupe.EndDateTime = endTime.ToString(format);
                                endTime = DateTime.Parse(field);
                                dedupe.EndDateTime = ConvertLocaltoUTC(endTime, timeZone).ToString(format);
                            }
                        }

                        if (fieldName == "Notes")
                            notes = field;   

                        var fieldID = folderFields[i].FieldID;
                        i++;

                        CCFieldValue objFieldValue = new CCFieldValue();
                        objFieldValue.FieldID = fieldID;
                        if (fieldName == "Start Time") { objFieldValue.Value = dedupe.StartDateTime; }
                        else if (fieldName == "End Time") { objFieldValue.Value = dedupe.EndDateTime; }
                        else
                        {
                            if (field == " ") objFieldValue.Value = String.Empty;
                            else objFieldValue.Value = field;
                        }
                        objFieldValue.ItemID = contectID;
                        objFieldValue.LastUpdated = DateTime.UtcNow;
                        objFieldValue.AccountGUID = accountGUID;
                        ObjFieldValues.Add(objFieldValue);
                    }
                }
                savedfields = CCFieldValueRepository.SaveFieldsObjValues(ObjFieldValues);
            }
            if (savedfields.Count() > 0)
            {
                bool res = UpdateContact(dedupe, contectID, type, notes);
                return true;
            }
            else return false;
        }

        private bool UpdateContact(AddDedupeViewModel dedupe, long contectID, int type, string notes)
        {
            CCItems contact = new CCItems();
            contact.ItemID = contectID; 
            contact.Notes = notes;
            contact.TextBody = notes;
            if (type == 1) { contact.DeDupeValue = dedupe.FirstName + "|" + dedupe.MiddleName + "|" + dedupe.LastName + "|" + dedupe.CompanyName + "|" + dedupe.Email; }
            else { contact.DeDupeValue = dedupe.Subject + "|" + dedupe.StartDateTime + "|" + dedupe.EndDateTime; }
            bool res = CCItemRepository.UpdateContact(contact);
            return res;
        }


        private DateTime ConvertLocaltoUTC(DateTime convertTime, string timeZone)
        {
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(convertTime, tz);
            return utcTime;
        }

    }
}