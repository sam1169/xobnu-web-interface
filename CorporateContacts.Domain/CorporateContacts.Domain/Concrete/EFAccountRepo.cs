using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Concrete
{
    public class EFAccountRepo : IAccountRepo
    {
        private EFDbContextAccounts context = new EFDbContextAccounts();

        public IQueryable<Account> Accounts
        {
            get { return context.Accounts; }
            set { }
        }

        public Account SaveAccount(Account accObj)
        {
            if (accObj.ID == 0)
            {
                context.Accounts.Add(accObj);
                context.SaveChanges();
            }
            else
            {
                var acc = context.Accounts.FirstOrDefault(a => a.ID == accObj.ID);
                acc.AccountName = accObj.AccountName;
                acc.AdditionalDiscount = accObj.AdditionalDiscount;
                acc.PlanID = accObj.PlanID;
                //acc.ConnectionString = accObj.ConnectionString;
                acc.TablePrefix = accObj.TablePrefix;
                acc.StripeCustomerID = accObj.StripeCustomerID;
                acc.LastModifiedDate = DateTime.Now;
                acc.HasPurchased = accObj.HasPurchased;
                acc.isPaymentIssue = accObj.isPaymentIssue;
                acc.isOverFlow = accObj.isOverFlow;
                context.SaveChanges();
            }
            return accObj;
        }

        public bool UpdateAccountInfo(Account accObj)
        {
            Account acc = context.Accounts.FirstOrDefault(a => a.ID == accObj.ID);
            if (acc != null)
            {
                acc.AccountName = accObj.AccountName;
                acc.AdditionalDiscount = accObj.AdditionalDiscount;
                acc.PlanID = accObj.PlanID;
               // acc.ConnectionString = accObj.ConnectionString;
                acc.TablePrefix = accObj.TablePrefix;
                acc.StripeCustomerID = accObj.StripeCustomerID;
                acc.BusinessAddress = accObj.BusinessAddress;
                acc.TimeZone = accObj.TimeZone;
                acc.LastModifiedDate = DateTime.Now;
                acc.HasPurchased = accObj.HasPurchased;
                acc.isOverFlow = accObj.isOverFlow;
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool setAccountStatus(bool status, long accID)
        {
            Account acc = context.Accounts.FirstOrDefault(a => a.ID == accID);
            if (acc != null)
            {
                acc.isOverFlow = status;
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
            
            
        }

        public bool ActivateTrialAccount(Account accObj)
        {
            Account acc = context.Accounts.FirstOrDefault(a => a.ID == accObj.ID);
            if (acc != null)
            {
                if (acc.TrialEnds == null)
                { 
                    acc.TrialEnds = DateTime.Now.AddDays(CCGlobalValues.trialPeriod);
                    context.SaveChanges();
                    return true;
                }
                else
                { 
                    return false;
                }                
            }
            else
            {
                return false;
            }
        }

        public bool DeleteAccount(long id)
        {
            Account dbEntry = context.Accounts.Find(id);
            if (dbEntry != null)
            {
                context.Accounts.Remove(dbEntry);
                context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
