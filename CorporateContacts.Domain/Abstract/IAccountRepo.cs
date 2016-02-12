using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Abstract
{
    public interface IAccountRepo
    {
        IQueryable<Account> Accounts { get; set; }

        Account SaveAccount(Account accountObj);

        bool DeleteAccount(long id);

        bool UpdateAccountInfo(Account accObj);

        bool ActivateTrialAccount(Account accObj);

        bool setAccountStatus(Boolean status, long accID);

    }
}
