using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.WebUI.Infrastructure
{
    public interface IAuthProvider
    {
        bool Authenticate(string email, string password,IUserRepo userRep);

        void SignOut();

        
    }
}
