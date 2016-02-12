using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Abstract
{
   public interface ICCTokenRepo
    {
        IQueryable<CCToken> CCTokens { get; set; }
        CCToken SaveToken(CCToken TokenObj);
        bool DeleteToken(long TokenId);
    }
}
