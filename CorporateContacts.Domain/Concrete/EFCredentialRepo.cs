using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Concrete
{
    public class EFCredentialRepo : ICredentialRepo
    {
        EFDBContextClient context = new EFDBContextClient();

        public IQueryable<Credential> Credentials
        {
            get
            {
                return context.Credentials;
            }
            set { }
        }

        public void SetConnectionString(string connStr)
        {
            this.context.ConnStr = connStr;
        }

        public Credential SaveCredential(Credential cred)
        {
            if (cred.ID == 0)
            {
                this.context.Credentials.Add(cred);
                this.context.SaveChanges();
            }
            else
            {
                Credential dbEntry = context.Credentials.Find(cred.ID);
                if (dbEntry != null)
                {
                    dbEntry.Name = cred.Name;
                    dbEntry.URL = cred.URL;
                    dbEntry.EmailAddress = cred.EmailAddress;
                    dbEntry.ServerVersion = cred.ServerVersion;
                    dbEntry.IsHostedExchange = cred.IsHostedExchange;
                    dbEntry.UseImpersonation = cred.UseImpersonation;
                    dbEntry.Password = cred.Password;
                    context.SaveChanges();
                }
            }
            return cred;
        }
      

        public bool DeleteCredential(long id)
        {
            Credential  dbEntry = context.Credentials.Find(id);
            if (dbEntry != null)
            {
                context.Credentials.Remove(dbEntry);
                context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
