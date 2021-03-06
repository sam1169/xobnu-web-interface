﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Abstract
{
    public interface IFeatureRepo
    {
        IQueryable<Feature> Features { get; set; }

        IQueryable<Feature> GetFeaturesForPlan(long planLevel);

        IQueryable<PurchasedFeatureDetails> GetFeaturesForOtherPlans(int planID);

    }
}
