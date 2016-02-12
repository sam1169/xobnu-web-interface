using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Abstract
{
    public interface IFolderRepo
    {
        IQueryable<Folder> Folders { get; set; }

        Folder SaveFolder(Folder folder);

        Subscription SaveSubscription(Subscription sub);

        List<Folder> GetPrimarySourcesForAccount(long accountID);

        List<SubscriptionDto> GetSubscriptionForPrimarySource(long primarySourceID, long accountID);

        string GetFolderNameFromID(long folderId);

        List<SubscriptionDto> GetAllSubscriptions(long accountID);
    }
}
