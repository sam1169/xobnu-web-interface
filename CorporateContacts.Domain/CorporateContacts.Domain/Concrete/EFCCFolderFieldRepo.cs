using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Concrete
{
    public class EFCCFolderFieldRepo : ICCFolderFieldRepo
    {
        private EFDBContextClient context = new EFDBContextClient();
        ICCConnectionsRepo CCConnectionRepository;
        ICCFolderRepo CCfolderRepo;


        public EFCCFolderFieldRepo(ICCConnectionsRepo conRepo , ICCFolderRepo foldRepo)
        { 
            CCConnectionRepository = conRepo;
            CCfolderRepo = foldRepo;
        }


        public IQueryable<CCFolderField> CCFolderFields
        {
            get { return context.CCFolderFields; }
            set { }
        }

        public CCFolderField SaveFolderFields(CCFolderField fieldObj)
        {
            if (fieldObj.FieldID == 0)
            {
                context.CCFolderFields.Add(fieldObj);
                context.SaveChanges();

                //Schedule Force ReSync

                List<CCConnection> connList = new List<CCConnection>();
                connList = CCConnectionRepository.CCSubscriptions.Where(con => con.FolderID == fieldObj.FolderID).ToList();

                foreach (var con in connList)
                {
                    CCConnection conEntry = context.CCSubscriptions.Find(con.ConnectionID);
                    conEntry.ResetAtNextSync = true;
                    context.SaveChanges();                    
                }
            }
            else
            {
                CCFolderField dbEntry = context.CCFolderFields.Find(fieldObj.FieldID);
                if (dbEntry != null)
                {
                    dbEntry.FieldCaption = fieldObj.FieldCaption;
                    dbEntry.FieldName = fieldObj.FieldName;
                    dbEntry.FieldType = fieldObj.FieldType;
                    dbEntry.Restriction = fieldObj.Restriction;
                    dbEntry.RestrictionValues = fieldObj.RestrictionValues;
                    dbEntry.isActive = fieldObj.isActive;
                    context.SaveChanges();
                }

                List<CCConnection> connList = new List<CCConnection>();
                connList = CCConnectionRepository.CCSubscriptions.Where(con => con.FolderID == fieldObj.FolderID).ToList();

                foreach (var con in connList)
                {
                    CCConnection conEntry = context.CCSubscriptions.Find(con.ConnectionID);
                    conEntry.ResetAtNextSync = true;
                    context.SaveChanges();
                }

            }
            return fieldObj;
        }

        public List<CCFolderField> SaveFolderFieldsObj(List<CCFolderField> folderObj)
        {
            long folderID=0;

            foreach (CCFolderField fieldvalues in folderObj)
            {
                context.CCFolderFields.Add(fieldvalues);
                folderID = fieldvalues.FolderID;
            }
            context.SaveChanges();

            List<CCConnection> connList = new List<CCConnection>();
            connList = CCConnectionRepository.CCSubscriptions.Where(con => con.FolderID == folderID).ToList();

            foreach (var con in connList)
            {
                CCConnection conEntry = context.CCSubscriptions.Find(con.ConnectionID);
                conEntry.ResetAtNextSync = true;
                context.SaveChanges();
            }


            return folderObj;

            //Schedule Force ReSync

            
        }

        public bool DeleteField(long id)
        {
            CCFolderField dbEntry = context.CCFolderFields.Find(id);
            if (dbEntry != null)
            {
                context.CCFolderFields.Remove(dbEntry);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public void SetConnectionString(string connStr)
        {
            this.context.ConnStr = connStr;
        }

        public bool IsFieldAvailable(string fname, long folderid)
        {

            var isAvailable = this.context.CCFolderFields
                             .Where(fid => fid.FolderID == folderid && fid.FieldName == fname)
                             .ToList();

            if (isAvailable.Count == 0)
            {
                return true;
            }
            else return false;

        }

        public List<string> IsAvailableField(List<string> fieldobj, long folderid)
        {

            var isavailable = this.context.CCFolderFields.Where(fid => fid.FolderID == folderid).ToList();

            List<string> needtoadd = new List<string>();
            foreach (var item in fieldobj)
            {
                var _isthere = isavailable.FirstOrDefault(fname => fname.FieldCaption == item);

                if (_isthere == null)
                {
                    needtoadd.Add(item);
                }
            }

            return needtoadd;

        }

        public void DeleteFolderFields(long folderID)
        {
            List<CCFolderField> folderFields = context.CCFolderFields.Where(ff => ff.FolderID == folderID).ToList();
            foreach (var folderField in folderFields)
            {
                CCFolderField dbEntry = context.CCFolderFields.Find(folderField.FieldID);
                if (dbEntry != null)
                {
                    context.CCFolderFields.Remove(dbEntry);
                }
            }

            context.SaveChanges();

        }

    }
}
