using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Concrete
{
    public class EFPlanRepo : IPlanRepo
    {
        private EFDbContextAccounts context;
        public EFPlanRepo()
        {
            context = new EFDbContextAccounts();
        }

        public IQueryable<Plan> Plans
        {
            get { return context.Plans; }
            set { }
        }
    }
}
