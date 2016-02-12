using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Concrete
{
    public class EFCCFolderRepo : ICCFolderRepo
    {
        private EFDBContextClient context = new EFDBContextClient();

        //ICCConnectionsRepo CCConnectionRepository;
        //IAccountRepo CCAccRepository;
        //ICCErrorLogRepo CCErrorLogRepository;
        
        //ICCItemRepo CCItemRepository;

        //ICCFolderFieldRepo CCFieldRepository;
        //ICCFieldValuesRepo CCFieldValueRepository;
        //ICCGroupRepo CCGroupRepository;
        //ICCGroupFieldRepo CCGroupFieldRepository;
        //ICCLayoutRepo CCLayoutRepository;
        //ICCLayoutGroupRepo CCLayoutGroupRepository;
        //ICredentialRepo CCCredentialRepository;
        //ICCFieldMappingsRepo CCFieldMappingsRepository;
        //ICCNoteRepo CCNoteRepository;

        //ICCSyncFieldsRepo CCSyncFieldsRepository;
        //ICCSyncItems CCSyncItemsRepository;

        //public EFCCFolderRepo(ICCConnectionsRepo connection, IAccountRepo account, ICCErrorLogRepo errLog,  ICCItemRepo items,
        //    ICCFolderFieldRepo field,ICCFieldValuesRepo fieldvalue, ICCGroupRepo group, ICCGroupFieldRepo groupfield, ICCLayoutRepo layout, ICCLayoutGroupRepo layoutgroup, 
        //    ICredentialRepo credential,ICCFieldMappingsRepo fieldmapping,ICCNoteRepo note, ICCSyncFieldsRepo syncFields, ICCSyncItems syncItems)
        //{
        //    CCConnectionRepository = connection;
        //    CCAccRepository = account;
        //    CCErrorLogRepository = errLog;
        //    CCItemRepository = items;
        //    CCFieldRepository = field;
        //    CCFieldValueRepository = fieldvalue;
        //    CCGroupRepository = group;
        //    CCGroupFieldRepository = groupfield;
        //    CCLayoutRepository = layout;
        //    CCLayoutGroupRepository = layoutgroup;
        //    CCCredentialRepository = credential;
        //    CCFieldMappingsRepository = fieldmapping;
        //    CCNoteRepository = note;
        //    CCSyncFieldsRepository = syncFields;
        //    CCSyncItemsRepository = syncItems;
        //}

        public IQueryable<CCFolder> CCFolders
        {
            get { return context.CCFolders; }
            set { }
        }

        public bool UpdatePauseSync(CCFolder folderObj)
        {
            CCFolder dbEntry = context.CCFolders.Find(folderObj.FolderID);
            if (dbEntry != null)
            {               
                if (folderObj.IsPaused != null)
                {
                    dbEntry.IsPaused = folderObj.IsPaused;
                }
                context.SaveChanges();
            }
            return true;
        }

        public CCFolder SaveFolder(CCFolder folderObj)
        {            
            var xx = folderObj.Name;

            if (folderObj.FolderID == 0)
            {
                if (folderObj.Type == 3)
                    folderObj.Type = 2;

                context.CCFolders.Add(folderObj);
                context.SaveChanges();
            }
            else
            {

                CCFolder dbEntry = context.CCFolders.Find(folderObj.FolderID);
                if (dbEntry != null)
                {                    
                    dbEntry.Name = folderObj.Name;
                    context.SaveChanges();
                }
            }

            return folderObj;
        }

        public bool DeleteFolder(long id)
        {
            CCFolder dbEntry = context.CCFolders.Find(id);
            if (dbEntry != null)
            {
                context.CCFolders.Remove(dbEntry);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public void SetConnectionString(string connStr)
        {
            this.context.ConnStr = connStr;
        }

        public CCFolder FolderDetails(long foilderID)
        {
            CCFolder folderDetails = context.CCFolders.Find(foilderID);
            if (folderDetails != null)
            {
                return folderDetails;
            }

            return null;
        }
    }
}
