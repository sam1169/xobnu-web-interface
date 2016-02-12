using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Concrete
{
    public class EFUserRepo : IUserRepo
    {
        private EFDbContextAccounts context = new EFDbContextAccounts();
        IAccountRepo accRepository;

        public EFUserRepo()
        {
            this.accRepository = new EFAccountRepo();
        }

        public EFUserRepo( IAccountRepo acc)
        {
            accRepository = acc;
        }

        public IQueryable<User> Users
        {
            get { return context.Users; }
            set { }
        }

        public User SaveUser(User userObj)
        {
            if (userObj.ID == 0)
            {
                context.Users.Add(userObj);
                context.SaveChanges();
            }
            else
            {
                User dbEntry = context.Users.Find(userObj.ID);
                if (dbEntry != null)
                {
                    dbEntry.FullName = userObj.FullName;
                    dbEntry.Email = userObj.Email;
                    dbEntry.Password = userObj.Password;
                    //dbEntry.UserType = userObj.UserType;
                    dbEntry.PrimaryUser = userObj.PrimaryUser;
                    dbEntry.ConfirmPassword = userObj.Password;
                    //dbEntry.GUID = userObj.GUID;
                    dbEntry.Status = userObj.Status;
                    dbEntry.LastModifiedDate = DateTime.Now;
                    dbEntry.isPasswordChange = userObj.isPasswordChange;
                    context.SaveChanges();
                }
            }
            return userObj;
        }

        public bool ActiveEmialVerification(User userObj)
        {
            User dbEntry = context.Users.Find(userObj.ID);
            if (dbEntry != null)
            {
                dbEntry.FullName = userObj.FullName;
                dbEntry.Email = userObj.Email;
                dbEntry.Password = userObj.Password;
                dbEntry.UserType = userObj.UserType;
                dbEntry.PrimaryUser = userObj.PrimaryUser;
                dbEntry.ConfirmPassword = userObj.Password;
                dbEntry.EmailVerified =true;
                dbEntry.Status = userObj.Status;
                dbEntry.LastModifiedDate = DateTime.Now;          
                context.SaveChanges();

                Account acc = context.Accounts.Where(a => a.ID == dbEntry.AccountID).FirstOrDefault();
                accRepository.ActivateTrialAccount(acc);

                return true;
            }
            return false;
        }

        public User GetUserByEmailAddress(string email)
        {
            return context.Users.FirstOrDefault(u => u.Email == email);
        }

        public User GetUserByGUID(string guidToSearch)
        {
            return context.Users.FirstOrDefault(u => u.GUID == guidToSearch);
        }

        public bool DeleteUser(long id)
        {
            User dbEntry = context.Users.Find(id);
            if (dbEntry != null)
            {
                context.Users.Remove(dbEntry);
                context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
