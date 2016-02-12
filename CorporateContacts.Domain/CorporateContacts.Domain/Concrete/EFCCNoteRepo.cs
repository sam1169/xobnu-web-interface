using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Concrete
{
    public class EFCCNoteRepo : ICCNoteRepo
    {
        private EFDBContextClient context = new EFDBContextClient();

        public IQueryable<CCNote> CCNotes
        {
            get { return context.CCNotes; }
            set { }
        }

        public CCNote SaveNote(CCNote noteObj)
        {
            if (noteObj.NotesID == 0)
            {
                context.CCNotes.Add(noteObj);
                context.SaveChanges();
            }
            else
            {
                CCNote dbEntry = context.CCNotes.Find(noteObj.NotesID);
                if (dbEntry != null)
                {
                    dbEntry.value = noteObj.value;
                    context.SaveChanges();
                }

            }
            return noteObj;
        }

        public void SetConnectionString(string connStr)
        {
            this.context.ConnStr = connStr;
        }


    }
}
