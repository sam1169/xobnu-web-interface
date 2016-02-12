using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Abstract
{
    public interface ICCConnectionsRepo
    {
        IQueryable<CCConnection> CCSubscriptions { get; set; }
        CCConnection SaveSubscription(CCConnection SubscObj);
        CCConnection SaveSubscriptionTag(CCConnection SubscObj);
        bool DeleteSubscription(long SubscId);
        void SetConnectionString(string connStr);
        void DeleteConnections(long folderID);
        bool UpdateSubscription(CCConnection SubscObj);
        bool UpdatePauseSync(CCConnection SubscObj);
        bool UpdateConnectionBooleanToggles(CCConnection SubscObj);
    }
}
