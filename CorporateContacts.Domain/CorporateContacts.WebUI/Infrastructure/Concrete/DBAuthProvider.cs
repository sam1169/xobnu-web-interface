using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;
using CorporateContacts.WebUI.Util;


namespace CorporateContacts.WebUI.Infrastructure.Concrete
{
    public class DBAuthProvider : IAuthProvider
    {
        private string rand = "00062008-0000-0000-C000-000000000046";

        public bool Authenticate(string email, string password, IUserRepo userRepo)
        {           
            // var user = userRepo.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            var user = userRepo.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                //string passwords = Encryption.DecryptStringAES(user.Password, rand);
                if (Encryption.DecryptStringAES(user.Password, rand) == password)
                {
                    FormsAuthentication.SetAuthCookie(email, false);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else return false;
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }


    }
}