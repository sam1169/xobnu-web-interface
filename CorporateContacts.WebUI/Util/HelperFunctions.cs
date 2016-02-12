using Xobnu.Domain.Abstract;
using Xobnu.Domain.Concrete;
using Xobnu.Domain.Entities;
using Xobnu.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Xobnu.WebUI.Util
{
    public class HelperFunctions
    {
        IAccountRepo accountRepo;
        IPlanRepo planRepository;
        IUserRepo userRepository;
        IPurchasedFeatureRepo purchRepository;
        ICCFolderRepo CCFolderRepository;
        ICCItemRepo CCItemRepository;
        IFeatureRepo featureRepository;
        ICCConnectionsRepo CCConnectionRepository;

        public HelperFunctions()
        {
            this.accountRepo = new EFAccountRepo();
            this.planRepository = new EFPlanRepo();
            this.userRepository = new EFUserRepo();
            this.purchRepository = new EFPurchasedProdRepo();
            this.CCFolderRepository = new EFCCFolderRepo();
            this.featureRepository = new EFFeatureRepo();
            this.CCConnectionRepository = new EFCCConnectionsRepo();
            this.CCItemRepository = new EFCCItemRepo();
        }

        public HelperFunctions(IAccountRepo account, IPlanRepo plan, IUserRepo user, IPurchasedFeatureRepo purch, ICCFolderRepo folder, IFeatureRepo feature, ICCConnectionsRepo CCConnection, ICCItemRepo CCItem)
        {
            accountRepo = account;
            planRepository = plan;
            userRepository = user;
            purchRepository = purch;
            CCFolderRepository = folder;
            featureRepository = feature;
            CCConnectionRepository = CCConnection;
            CCItemRepository = CCItem;
        }


        public LimitationsViewModel updateAccountLimitations(Account accountObj)
        {
            LimitationsViewModel limitationsObj = new LimitationsViewModel();
            var accDetails = accountRepo.Accounts.Where(aguid => aguid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();
            var planLeval = planRepository.Plans.Where(pid => pid.ID == accDetails.PlanID).FirstOrDefault().PlanLevel;
            var featureQuality = featureRepository.Features.Where(pid => pid.PlanLevel == planLeval & pid.Type == "Max Items per Folder").FirstOrDefault();
            var savedQuality = purchRepository.Purchases.Where(fid => fid.FeatureID == featureQuality.ID && fid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();

            var folderList = CCFolderRepository.CCFolders.Where(f => f.AccountGUID == accountObj.AccountGUID).ToList();
            limitationsObj.folderList = new List<FolderDetailsWithItemCount>();
            foreach (var fold in folderList)
            {
                FolderDetailsWithItemCount foldC = new FolderDetailsWithItemCount();
                foldC.fold = fold;
                foldC.itemCount = CCItemRepository.CCContacts.Where(i => i.FolderID == fold.FolderID & i.isDeleted == false).Count();
                limitationsObj.folderList.Add(foldC);
            }

            if (savedQuality != null)
            {
                var quantitySaved = (savedQuality.Quantity) / (featureQuality.Quantity);
                limitationsObj.purchasedConnectionCount = (int)(quantitySaved * 5);
                limitationsObj.availableCconnectionCount = (int)(quantitySaved * 5) - (int)CCConnectionRepository.CCSubscriptions.Where(C => C.AccountGUID == accountObj.AccountGUID).Count();
                limitationsObj.maxItemCountPerFolder = featureQuality.Quantity;
                if (featureRepository.Features.Where(pid => pid.PlanLevel == planLeval & pid.Type == "Sync Calendar").FirstOrDefault().Quantity == 0)
                    limitationsObj.isCalendarSyncAvailable = false;
                else
                    limitationsObj.isCalendarSyncAvailable = true;


            }

            return limitationsObj;
        }


        public List<NotificationListViewModel> generateNotificationList(Account accountObj)
        {
            List<NotificationListViewModel> notificationList = new List<NotificationListViewModel>();

            //Get Trial Period Details
            DateTime trialEndDate = (DateTime)accountRepo.Accounts.FirstOrDefault(a => a.AccountGUID == accountObj.AccountGUID).TrialEnds;
            double timeRemaining = (trialEndDate - DateTime.Now.Date).TotalDays;

            NotificationListViewModel NF1 = new NotificationListViewModel();

            if(accountObj.HasPurchased != true)
            {
                if (timeRemaining > 0)
                    NF1.NotificationMsg = "Your trial account expires in " + timeRemaining + " Days.";
                else
                    NF1.NotificationMsg = "Your account has been expired. Please purchase to continue using Corporate Contacts";
                NF1.url = "/Admin/BillingOptions/1";
                NF1.NotificationType = "Account";
                NF1.priority = 10;

                notificationList.Add(NF1);
            }
            

            //Get No. of Connection Details & Get Folder Item Count Details
            var accDetails = accountRepo.Accounts.Where(aguid => aguid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();
            //var planLeval = planRepository.Plans.Where(pid => pid.ID == accDetails.PlanID).FirstOrDefault().PlanLevel;
            //modified
            var planLeval = 10;
            var featureQuality = featureRepository.Features.Where(pid => pid.PlanLevel == planLeval & pid.Type == "Max Items per Folder").FirstOrDefault();
            var savedQuality = purchRepository.Purchases.Where(fid => fid.FeatureID == featureQuality.ID && fid.AccountGUID == accountObj.AccountGUID).FirstOrDefault();
            if (savedQuality != null)
            {
                var quantitySaved = (savedQuality.Quantity) / (featureQuality.Quantity);
                var purchasedConnectionCount = (int)(quantitySaved * 5);
                var availableConnectionCount = (int)(quantitySaved * 5) - (int)CCConnectionRepository.CCSubscriptions.Where(C => C.AccountGUID == accountObj.AccountGUID).Count();
               

                if (((((double)purchasedConnectionCount - (double)availableConnectionCount) / (double)purchasedConnectionCount) * 100) > 70)
                {
                    NotificationListViewModel NF2 = new NotificationListViewModel();
                    NF2.NotificationMsg = "You have used " + ((double)purchasedConnectionCount - (double)availableConnectionCount) + " of your " + ((double)purchasedConnectionCount) + " account Connections.";
                    NF2.url = "/Admin/BillingOptions/1";
                    NF2.NotificationType = "Account";
                    NF2.priority = 7;

                    notificationList.Add(NF2);
                }

                var maxItemCountPerFolder = featureQuality.Quantity;

                var folderList = CCFolderRepository.CCFolders.Where(f => f.AccountGUID == accountObj.AccountGUID).ToList();

                NotificationListViewModel[] NFArray = new NotificationListViewModel[folderList.Count];
                int c = 0;

                foreach (var fold in folderList)
                {                    
                    var itemCount = CCItemRepository.CCContacts.Where(i => i.FolderID == fold.FolderID & i.isDeleted == false).Count();
                    if (((((double)itemCount) / (double)maxItemCountPerFolder) * 100) > 70)
                    {
                        NFArray[c] = new NotificationListViewModel();
                        NFArray[c].NotificationMsg = "Folder '" + fold.Name + "' has almost reached its Max Item Count.";
                        NFArray[c].url = "/Admin/BillingOptions/1";
                        NFArray[c].NotificationType = "Account";
                        NFArray[c].priority = 5;
                        c++;
                    }
                    
                }

                foreach (var noti in NFArray)
                {
                    if(noti != null)
                        notificationList.Add(noti);
                }
            }
            return notificationList;
        }

        public void CheckAcccountStatus(Account accountObj)
        {
            var folderList = CCFolderRepository.CCFolders.Where(f => f.AccountGUID == accountObj.AccountGUID).ToList();

            accountObj.isOverFlow = false;

            foreach (var fold in folderList)
            {
                int FolderItemCount = CCItemRepository.CCContacts.Where(i => i.FolderID == fold.FolderID).Count();

                LimitationsViewModel limitationsObj = new LimitationsViewModel();
                HelperFunctions HF = new HelperFunctions();
                limitationsObj = HF.updateAccountLimitations(accountObj);

                if ((FolderItemCount > limitationsObj.maxItemCountPerFolder) | (fold.isOverFlow == true))
                {
                    accountObj.isOverFlow = true;
                }
            }
            accountRepo.SaveAccount(accountObj);
        }

    }
}