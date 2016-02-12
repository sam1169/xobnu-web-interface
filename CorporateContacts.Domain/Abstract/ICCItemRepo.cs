using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Abstract
{
    public interface ICCItemRepo
    {
        IQueryable<CCItems> CCContacts { get; set; }
        CCItems SaveContact(CCItems ContactObj);
        bool UpdateContact(CCItems contactObj);
        bool DeleteContact(long ContactId);
        void SetConnectionString(string connStr);
        List<CCContactWithFields> GetContacts(long ID, int pageSize, int pageNumber, List<CCFolderField> selectedcontactsFields);
        long CreateContact(long fid, string augid);
        JqGridModel GetJQModel(int pageNumber, int recordsPerPage, string sortColumn, string sortOrder, string sortBy, long folderID, int type, string searchField, string searchValue, string timeZone,string accGUID);
        void UpdateContactTable();
        void DeleteItems(long folderID);
    }
}
