﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Abstract
{
   public interface ICCNoteRepo
    {
        IQueryable<CCNote> CCNotes { get; set; }
       CCNote SaveNote (CCNote NoteObj);       
       void SetConnectionString(string connStr);
    }
}
