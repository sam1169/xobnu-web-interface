﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Abstract
{
   public interface ICCGroupFieldRepo
    {
        IQueryable<CCGroupField> CCGroupsFields { get; set; }
        CCGroupField SaveGroupField(CCGroupField GroupObj);
        bool DeleteGroupField(long GroupId);
        void SetConnectionString(string connStr);
        bool DeleteFieldsByGroup(long groupid);
    }
}
