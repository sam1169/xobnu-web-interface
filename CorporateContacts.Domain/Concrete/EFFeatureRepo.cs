using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Concrete
{
    public class EFFeatureRepo : IFeatureRepo
    {
        private EFDbContextAccounts context = new EFDbContextAccounts();
     
        public IQueryable<Feature> Features
        {
            get { return context.Features; }
            set { }
        }

        public IQueryable<Feature> GetFeaturesForPlan(long planLevel)
        {
            return context.Features.Where(x => x.PlanLevel == planLevel);
        }

        public IQueryable<PurchasedFeatureDetails> GetFeaturesForOtherPlans(int planID)
        {        
            
            var productsForAccount = from pl in context.Plans.Where(x => x.ID != planID)
                                     join f in context.Features.Where(f=>f.Visible) on pl.PlanLevel equals f.PlanLevel
                                     select new PurchasedFeatureDetails { Name = f.Name, Description = f.Description, PlanName = pl.Name };
            return productsForAccount;

        }
    }
}
