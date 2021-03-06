﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Abstract
{
    public interface IFolderFieldRepo
    {
        IQueryable<FolderField> FolderFields { get; set; }

        FolderField SaveFolderField(FolderField folderfield);
    }
}
