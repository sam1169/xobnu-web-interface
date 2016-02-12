using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Concrete
{
    public class EFFolderRepo : IFolderRepo
    {
        private EFDbContextAccounts context = new EFDbContextAccounts();

        public IQueryable<Folder> Folders { get { return context.Folders; } set { } }

        public Folder SaveFolder(Folder folder)
        {
            if (folder.ID == 0)
            {
                context.Folders.Add(folder);
                context.SaveChanges();
            }
            return folder;
        }

        public Subscription SaveSubscription(Subscription subscription)
        {
            if (subscription.ID == 0)
            {
                context.Subscriptions.Add(subscription);
                context.SaveChanges();
            }
            return subscription;
        }

        public List<Folder> GetPrimarySourcesForAccount(long accountID)
        {
            return context.Folders.Where(x => x.IsMaster == true && x.AccountID == accountID).ToList();
        }

        public List<SubscriptionDto> GetSubscriptionForPrimarySource(long primarySourceID, long accountID)
        {
            var query = from s in context.Subscriptions
                        from f in context.Folders
                        where s.PrimaryID == primarySourceID && s.AccountID == accountID
                        where s.SubscriberID == f.ID
                        select new SubscriptionDto
                        {
                            AllowAdditions = s.AllowAdditions,                           
                            AccountID = s.AccountID,
                            IgnoreExisting = s.IgnoreExisting,
                            SubscriberID = s.SubscriberID,
                            PrimaryID = s.PrimaryID,
                            SubscribingFolder = f,
                            ID = s.ID
                        };
            return query.ToList();
        }

        public string GetFolderNameFromID(long folderId)
        {
            var obj = context.Folders.FirstOrDefault(x => x.ID == folderId);
            if (obj != null) return obj.FolderName;
            else return "";
        }


        public List<SubscriptionDto> GetAllSubscriptions(long accountID)
        {
            var query = from s in context.Subscriptions
                        from f in context.Folders
                        join f2 in context.Folders on s.PrimaryID equals f2.ID
                        where s.AccountID == accountID
                        where s.SubscriberID == f.ID
                        select new SubscriptionDto
                        {
                            AllowAdditions = s.AllowAdditions,                            
                            AccountID = s.AccountID,
                            IgnoreExisting = s.IgnoreExisting,
                            SubscriberID = s.SubscriberID,
                            PrimaryID = s.PrimaryID,
                            PrimaryName = f2.FolderName,
                            SubscribingFolder = f,
                            ID = s.ID,
                            LastSync = s.LastSync,
                            Status = s.Status
                        };
            return query.ToList();
        }




    }
}
