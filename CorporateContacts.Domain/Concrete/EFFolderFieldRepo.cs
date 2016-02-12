using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Concrete
{
   public class EFFolderFieldRepo : IFolderFieldRepo
    {
        EFDbContextAccounts context = new EFDbContextAccounts();
        public IQueryable<FolderField> FolderFields
        {
            get { return context.FolderFields; }
            set { }
        }

        public FolderField SaveFolderField(FolderField folderfield)
        {
            if (folderfield.ID == 0)
            {
                context.FolderFields.Add(folderfield);
                context.SaveChanges();
            }
            else
            {
                var ff = context.FolderFields.FirstOrDefault(a => a.ID == folderfield.ID);
                ff.AccountID = folderfield.AccountID;
                ff.FType = folderfield.FType;
                ff.FName = folderfield.FName;
                ff.FolderID = folderfield.FolderID;
                ff.RealName = folderfield.RealName;
                ff.MappedFieldID = folderfield.MappedFieldID;
                ff.TType = folderfield.TType;
                context.SaveChanges();
            }
            return folderfield;
        }
    }
}
