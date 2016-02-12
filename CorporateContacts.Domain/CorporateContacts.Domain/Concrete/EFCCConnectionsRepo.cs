using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;


namespace CorporateContacts.Domain.Concrete
{
    public class EFCCConnectionsRepo : ICCConnectionsRepo
    {
        private EFDBContextClient context = new EFDBContextClient();

        public IQueryable<CCConnection> CCSubscriptions
        {
            get { return context.CCSubscriptions; }
            set { }
        }

        public bool UpdatePauseSync(CCConnection SubscObj)
        {
            CCConnection dbEntry = context.CCSubscriptions.Find(SubscObj.ConnectionID);
            if (dbEntry != null)
            {
                if (SubscObj.IsPaused != null)
                {
                    dbEntry.IsPaused = SubscObj.IsPaused;
                }
            }
            context.SaveChanges();

            return true;
        }

        public bool UpdateConnectionBooleanToggles(CCConnection SubscObj)
        {
            CCConnection dbEntry = context.CCSubscriptions.Find(SubscObj.ConnectionID);
            if (dbEntry != null)
            {
                if (SubscObj.AllowAdditions != null)
                {
                    dbEntry.AllowAdditions = SubscObj.AllowAdditions;
                }
                if (SubscObj.IgnoreExisting != null)
                {
                    dbEntry.IgnoreExisting = SubscObj.IgnoreExisting;
                }
                if (SubscObj.CategoryFilterUsed != null)
                {
                    dbEntry.CategoryFilterUsed = SubscObj.CategoryFilterUsed;
                }
                if (SubscObj.CopyPhotos != null)
                {
                    dbEntry.CopyPhotos = SubscObj.CopyPhotos;
                }
                if (SubscObj.TurnOffReminders != null)
                {
                    dbEntry.TurnOffReminders = SubscObj.TurnOffReminders;
                }
                if (SubscObj.tagSubject != null)
                {
                    dbEntry.tagSubject = SubscObj.tagSubject;
                }
                if (SubscObj.IsRunning != null)
                {
                    dbEntry.IsRunning = SubscObj.IsRunning;
                }
                if (SubscObj.IsPaused != null)
                {
                    dbEntry.IsPaused = SubscObj.IsPaused;
                }

                if (SubscObj.LastSyncTime != null)
                {
                    dbEntry.LastSyncTime = SubscObj.LastSyncTime;
                }
                context.SaveChanges();
            }
            return true;
        }

        public CCConnection SaveSubscription(CCConnection SubscObj)
        {
            if (SubscObj.ConnectionID == 0)
            {
                context.CCSubscriptions.Add(SubscObj);
                context.SaveChanges();
            }
            else
            {
                CCConnection dbEntry = context.CCSubscriptions.Find(SubscObj.ConnectionID);
                if (dbEntry != null)
                {
                    dbEntry.AllowAdditions = dbEntry.AllowAdditions;
                    dbEntry.IgnoreExisting = SubscObj.IgnoreExisting;
                    dbEntry.CategoryFilterUsed = SubscObj.CategoryFilterUsed;
                    dbEntry.CopyPhotos = SubscObj.CopyPhotos;
                    dbEntry.TurnOffReminders = SubscObj.TurnOffReminders;
                    dbEntry.CategoryFilterValues = SubscObj.CategoryFilterValues;
                    dbEntry.SyncDirection = SubscObj.SyncDirection;
                    context.SaveChanges();
                }
            }
            return SubscObj;
        }

        public CCConnection SaveSubscriptionTag(CCConnection SubscObj)
        {
            CCConnection dbEntry = context.CCSubscriptions.Find(SubscObj.ConnectionID);
            if (dbEntry != null)
            {
                dbEntry.Tag = SubscObj.Tag;
                context.SaveChanges();
            }
            return SubscObj;
        }

        public bool UpdateSubscription(CCConnection SubscObj)
        {
            CCConnection dbEntry = context.CCSubscriptions.Find(SubscObj.ConnectionID);
            if (dbEntry != null)
            {

                dbEntry.SubjectTag = SubscObj.SubjectTag;
                dbEntry.tagSubject = SubscObj.tagSubject;
                dbEntry.AllowAdditions = dbEntry.AllowAdditions;
                dbEntry.IgnoreExisting = SubscObj.IgnoreExisting;
                dbEntry.CategoryFilterUsed = SubscObj.CategoryFilterUsed;
                dbEntry.CopyPhotos = SubscObj.CopyPhotos;
                dbEntry.TurnOffReminders = SubscObj.TurnOffReminders;
                dbEntry.CategoryFilterValues = SubscObj.CategoryFilterValues;
                dbEntry.SyncDirection = SubscObj.SyncDirection;
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DeleteSubscription(long id)
        {
            CCConnection dbEntry = context.CCSubscriptions.Find(id);
            if (dbEntry != null)
            {
                context.CCSubscriptions.Remove(dbEntry);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public void SetConnectionString(string connStr)
        {
            this.context.ConnStr = connStr;
        }

        public void DeleteConnections(long folderID)
        {
            List<CCConnection> connections = context.CCSubscriptions.Where(cid => cid.FolderID == folderID).ToList();
            foreach (var connection in connections)
            {
                CCConnection dbEntry = context.CCSubscriptions.Find(connection.ConnectionID);
                if (dbEntry != null)
                {
                    // get and remove mapping field
                    var mappingFields = context.CCFieldMappings.Where(cid => cid.ConnectionID == connection.ConnectionID).ToList();
                    foreach (var mappingfield in mappingFields)
                    {
                        CCFieldMapping dbfm = context.CCFieldMappings.Find(mappingfield.ID);
                        if (dbfm != null)
                        {
                            context.CCFieldMappings.Remove(dbfm);
                        }
                    }

                    context.CCSubscriptions.Remove(dbEntry);
                }
            }
            context.SaveChanges();
        }
    }
}
