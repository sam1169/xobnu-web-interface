using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CorporateContacts.Domain.Entities
{
    [Table("tblConnections")]
    public class CCConnection
    {
        [Key]
        public long ConnectionID { get; set; }
        public long FolderID { get; set; }
        public string FolderName { get; set; }
        public string Owner { get; set; }
        public string Type { get; set; }
        //public bool TwoWay { get; set; }
        public bool AllowAdditions { get; set; }
        public bool IgnoreExisting { get; set; }
        public string SyncState { get; set; }
        public bool CategoryFilterUsed { get; set; }
        public string CategoryFilterValues { get; set; }
        public bool CopyPhotos { get; set; }
        public bool TurnOffReminders { get; set; }
        public long CredentialsID { get; set; }
        public string SourceID { get; set; }
        public string SourceID2 { get; set; }
        public string Tag { get; set; }
        public string LastSyncTime { get; set; }
        public string SyncDirection { get; set; }
        public bool? IsRunning { get; set; }
        //public string SyncStartTime { get; set; }
        public string CurrentSyncStarted { get; set; }
        public int Frequency { get; set; }
        public string AdminNotes { get; set; }
        public string SecondaryAccount { get; set; }
        public int AccessType { get; set; }
        public bool tagSubject { get; set; }
        public string SubjectTag { get; set; }
        public bool PicSyncIsRunning { get; set; }
        public string PicSyncState { get; set; }
        public string PicLastSyncTime { get; set; }
        public string AccountGUID { get; set; }
        public bool? IsPaused { get; set; }
        public bool ResetAtNextSync { get; set; }
       
    }
}
