using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;

namespace Xobnu.WebUI.Infrastructure
{
    public interface IAuthProvider
    {
        bool Authenticate(string email, string password,IUserRepo userRep);

        void SignOut();

        
    }
}
