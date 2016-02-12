using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateContacts.WebUI.Models
{
    public class SelectFolderViewModel
    {
        public List<TreeViewFolder> FolderList { get; set; }
        public string SelectedFolderId { get; set; }
        public string SelectedFolderName { get; set; }
        public string SelectedFolderOwnerInfo { get; set; }
        public bool AddingPrimary { get; set; }
        public long PrimarySourceId {get;set;}        
        public bool AllowAdditions { get; set; }
        public bool IgnoreExisting { get; set; }
        public long CredentialID { get; set; }
        public bool CategoryFilterUsed { get; set; }
        public bool CopyPhotos { get; set; }
        public bool TurnOffReminders { get; set; }
        public string UniqueId { get; set; }
        public int FolderType { get; set; }
        public string SyncDirection { get; set; }
        public string CategoryFilterValues { get; set; }
        public bool TagAllSubject { get; set; }
        public string SubjectTag { get; set; }
    }
}