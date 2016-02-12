using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Abstract
{
    public interface IUserRepo
    {
        IQueryable<User> Users { get; set; }

        User SaveUser(User userObj);

        User GetUserByEmailAddress(string email);

        User GetUserByGUID(string guid);

        bool DeleteUser(long userId);

        bool ActiveEmialVerification(User userObj);
    }
}
