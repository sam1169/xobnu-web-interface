using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Abstract
{
   public interface ICCFolderFieldRepo
    {
        IQueryable<CCFolderField> CCFolderFields { get; set; }
        CCFolderField SaveFolderFields(CCFolderField FolderFieldObj);
        List<CCFolderField> SaveFolderFieldsObj(List<CCFolderField> FolderFieldObj);
        bool DeleteField(long FolderId);
       // CCFolderField GetFieldsByFolderID(long FolderId);
        void SetConnectionString(string connStr);
        bool IsFieldAvailable(string fname, long folderid);
        List<string> IsAvailableField(List<string> fieldobj, long folderid);
        void DeleteFolderFields(long folderID);
    }
}
