using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Concrete
{
    public class EFCCFieldMappingRepo : ICCFieldMappingsRepo
    {
        private EFDBContextClient context = new EFDBContextClient();

        public IQueryable<CCFieldMapping> CCFieldsMapping
        {
            get { return context.CCFieldMappings; }
            set { }
        }

        public CCFieldMapping SaveFieldMapping(CCFieldMapping FieldMappingObj)
        {
            context.CCFieldMappings.Add(FieldMappingObj);
            context.SaveChanges();
            return FieldMappingObj;
        }

        public void SetConnectionString(string connStr)
        {
            this.context.ConnStr = connStr;
        }

        public bool DeleteMappingFields(long id)
        {
            CCFieldMapping dbEntry = context.CCFieldMappings.Find(id);
            if (dbEntry != null)
            {
                context.CCFieldMappings.Remove(dbEntry);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DeleteMappingFieldBySubscriptionID(long subscriptionID)
        {
            var savedfield = this.context.CCFieldMappings
                           .Where(sid => sid.ConnectionID == subscriptionID).ToList();

            foreach (var field in savedfield)
            {

                bool res = this.DeleteMappingFields(field.ID);
            }

            return true;
        }

        public bool SaveAllMappingFields(long id, long cid, string accountGUID)
        {
            CCFieldMapping fieldMapping = new CCFieldMapping();
            var folderFields = this.context.CCFolderFields.Where(fid => fid.FolderID == id).ToList();

            foreach (var ff in folderFields)
            {
                fieldMapping.ConnectionID = cid;
                fieldMapping.FieldName = ff.FieldName;
                fieldMapping.Caption = ff.FieldCaption;
                fieldMapping.MappedFieldID = ff.FieldID;
                fieldMapping.AccountGUID = accountGUID;
                var res = SaveFieldMapping(fieldMapping);
            }

            return true;
        }
    }
}
