using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateContacts.WebUI.Models
{
    public class MappingFieldsViewModel
    {
        public IEnumerable<String> AllMappingFields { get; set; }
        public IEnumerable<String> SavedMappingFields { get; set; }
        public long FolderID { get; set; }
        public long SubscriptionID { get; set; }
    }
}