using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Abstract
{
    public interface ICCHealthMsgs
    {
        IQueryable<CCHealthMsgs> CCHealthMsgs { get; set; }

        CCHealthMsgs SaveHealthMsg(CCHealthMsgs healthMsgObj);     
    }
}
