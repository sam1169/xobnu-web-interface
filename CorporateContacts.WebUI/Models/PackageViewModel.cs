using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xobnu.Domain.Entities;

namespace Xobnu.WebUI.Models
{
    public class PackageViewModel
    {
       public List<Plan> Plans { get; set; }
       public List<string> Types { get; set; }
       public List<List<string>> Names { get; set; }
       public int PlanID { get; set; }
       public Plan SelectedPlanDetails { get; set; }
       public int Amount { get; set; }
       public bool IsCardDetailsSaved { get; set; }
       public List<CardViewModel> CardDetails { get; set; }
       public string DefaultCardID { get; set; }
       public int QuantitySaved { get; set; }
       public string BillingPlan { get; set; }
       public TrialDataModel trialObj { get; set; }

       public PackageViewModel()
       {
           trialObj = new TrialDataModel();
       }
    }
}