using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace CorporateContacts.WebUI.Models
{
    public class ContactsViewModel
    {
        public IEnumerable<CorporateContacts.Domain.Entities.CCFolderField> ContactsFields { get; set; }
        public IEnumerable<CorporateContacts.Domain.Entities.CCContactWithFields> ContactDetails { get; set; }
        public IPagedList<CorporateContacts.Domain.Entities.CCItems> PageList { get; set; }
        public long FolderID { get; set; }
        public int PageID { get; set; }
        public string FolderName { get; set; }
        public int FolderType { get; set; }
    }
}