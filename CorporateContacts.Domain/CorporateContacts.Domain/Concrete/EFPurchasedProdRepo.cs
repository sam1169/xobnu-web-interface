using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Concrete
{
    public class EFPurchasedProdRepo : IPurchasedFeatureRepo
    {
        private EFDbContextAccounts context;
        public EFPurchasedProdRepo()
        {
            context = new EFDbContextAccounts();
        }
        public IQueryable<PurchasedFeatures> Purchases
        {
            get { return context.PurchasedFeatures; }
            set { }
        }

        public IQueryable<PurchasedFeatureDetails> GetFeaturesForAccount(string accountGUID)
        {
            var purchasesForAccount = context.PurchasedFeatures.Where(p => p.AccountGUID == accountGUID);
            var productsForAccount = from f in context.Features.Where(x=>x.Visible)
                                     join pr in purchasesForAccount on f.ID equals pr.FeatureID
                                     select new PurchasedFeatureDetails {  Name = f.Name, Description = f.Description,Enabled=pr.Enabled,Expiry=pr.ExpiryDate };
            return productsForAccount;
            
        }

        public PurchasedFeatures SavePurchase(PurchasedFeatures purchase)
        {
            if (purchase.ID == 0)
            {
                context.PurchasedFeatures.Add(purchase);
                context.SaveChanges();
            }
            return purchase;
        }

        public bool DeleteFPurchasedFeature(long id)
        {
            PurchasedFeatures  dbEntry = context.PurchasedFeatures.Find(id);
            if (dbEntry != null)
            {
                context.PurchasedFeatures.Remove(dbEntry);
                context.SaveChanges();
                return true;
            }
            return false;
        }


    }
}
