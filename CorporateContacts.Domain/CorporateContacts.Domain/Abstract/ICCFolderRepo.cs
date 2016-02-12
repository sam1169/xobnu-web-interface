using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Abstract
{
   public interface ICCFolderRepo
    {
        IQueryable<CCFolder > CCFolders { get; set; }
        CCFolder SaveFolder (CCFolder  FolderObj);
        bool DeleteFolder(long FolderId);
        void SetConnectionString(string connStr);
        CCFolder FolderDetails(long foilderID);
        bool UpdatePauseSync(CCFolder FolderObj);
    }
}
