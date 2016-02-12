using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Abstract
{
    public interface IPurchasedFeatureRepo
    {
        IQueryable<PurchasedFeatures> Purchases { get; set; }

        IQueryable<PurchasedFeatureDetails> GetFeaturesForAccount(string accountGUID);

        PurchasedFeatures SavePurchase(PurchasedFeatures purchase);

        bool DeleteFPurchasedFeature(long id);
    }
}
