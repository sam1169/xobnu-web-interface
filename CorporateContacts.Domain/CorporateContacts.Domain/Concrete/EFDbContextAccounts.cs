using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Entities;
using System.Configuration;

namespace CorporateContacts.Domain.Concrete
{
    public class EFDbContextAccounts : DbContext
    {
        public EFDbContextAccounts()
        {           

            string dbConnction = ConfigurationManager.ConnectionStrings["EFDBContextAccounts"].ConnectionString;
            this.Database.Connection.ConnectionString = dbConnction;
            this.Database.CommandTimeout = 1800;
        }

        public string ConnStr
        {
            get { return this.Database.Connection.ConnectionString; }
            set
            {
                this.Database.Connection.ConnectionString = value; 
                this.Database.CommandTimeout = 1800;
            }
        }

        public DbSet<Plan> Plans { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Feature> Features { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<PurchasedFeatures> PurchasedFeatures { get; set; }

        public DbSet<Folder> Folders { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<FolderField> FolderFields { get; set; }

        public DbSet<CCToken> CCTokens { get; set; }

    }

   
}
