using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Xobnu.WebUI.Models
{
    public class TrialDataModel
    {
        public DateTime? trialEndDate { get; set; }
        public Boolean hasPurchased { get; set; }
        public DateTime? createdDate { get; set; }
        public Boolean showPurchaseOptions { get; set; }
        public Boolean showAtStartup { get; set; }

        public TrialDataModel()
        {
            showPurchaseOptions = false;
            showAtStartup = true;
        }
    }
}