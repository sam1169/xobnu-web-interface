﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Abstract
{
   public interface ICCGroupRepo
    {
        IQueryable<CCGroup> CCGroups { get; set; }
        CCGroup SaveGroup(CCGroup GroupObj);
        bool DeleteGroup(long GroupId);
        void SetConnectionString(string connStr);
        CClayoutWithGroups GetLayoutGroupsfieldsAndValues(long lid, long cid, long grpid,string timeZone);
    }
}
