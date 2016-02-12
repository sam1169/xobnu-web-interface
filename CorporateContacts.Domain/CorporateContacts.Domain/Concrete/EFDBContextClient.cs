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
    public class EFDBContextClient : DbContext
    {
        public EFDBContextClient()
        {           

            string dbConnction = ConfigurationManager.ConnectionStrings["EFDBContextClient"].ConnectionString;
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

        public DbSet<Credential> Credentials { get; set; }

        public DbSet<CCFolder> CCFolders { get; set; }

        public DbSet<CCFolderField> CCFolderFields { get; set; }

        public DbSet<CCItems> CCContacts { get; set; }

        public DbSet<CCFieldValue> CCFieldValues { get; set; }

        public DbSet<CCGroup> CCGroups { get; set; }

        public DbSet<CCGroupField> CCGroupFields { get; set; }

        public DbSet<CCLayout> CCLayouts { get; set; }

        public DbSet<CCLayoutGroup> CCLayoutGroups { get; set; }

        public DbSet<CCConnection> CCSubscriptions { get; set; }

        public DbSet<CCFieldMapping> CCFieldMappings { get; set; }

        public DbSet<CCNote> CCNotes { get; set; }

        public DbSet<CCSyncFields> CCSyncFields { get; set; }

        public DbSet<CCSyncItems> CCSyncItems { get; set; }

        public DbSet<CCHistoryLog> CCHistoryLog { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CCSyncFields>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<CCSyncFields>().Property(x => x.Value).IsRequired();
            modelBuilder.Entity<CCSyncFields>().Property(x => x.Name).HasMaxLength(200);
            modelBuilder.Entity<CCSyncFields>().Property(x => x.Value).HasMaxLength(500);
            modelBuilder.Entity<CCSyncFields>().Property(x => x.Type).HasMaxLength(50);

            modelBuilder.Entity<CCItems>().Property(x => x.DeDupeValue).HasMaxLength(500);

            modelBuilder.Entity<CCSyncItems>().Property(x => x.DeDupeValue).HasMaxLength(200);
            modelBuilder.Entity<CCSyncItems>().Property(x => x.Tag).HasMaxLength(50);
            modelBuilder.Entity<CCSyncItems>().Property(x => x.RefID).HasMaxLength(300);
            modelBuilder.Entity<CCSyncItems>().Property(x => x.RecurrenceNo).IsOptional();
            modelBuilder.Entity<CCSyncItems>().Property(x => x.HasPicture).IsRequired();
            modelBuilder.Entity<CCSyncItems>().Property(x => x.HashCode).IsRequired();
            modelBuilder.Entity<CCSyncItems>().Property(x => x.PicID).IsRequired();

            modelBuilder.Entity<CCConnection>().Property(x => x.Frequency).IsRequired();
            modelBuilder.Entity<CCConnection>().Property(x => x.tagSubject).IsRequired();
            modelBuilder.Entity<CCConnection>().Property(x => x.SubjectTag).HasMaxLength(20);
            modelBuilder.Entity<CCConnection>().Property(x => x.PicSyncIsRunning).IsRequired();

            modelBuilder.Entity<CCFieldValue>().Property(x => x.Value2).HasMaxLength(200);

            modelBuilder.Entity<CCItems>().Property(x => x.HasPicture).IsRequired();
            modelBuilder.Entity<CCItems>().Property(x => x.HashCode).IsRequired();
            modelBuilder.Entity<CCItems>().Property(x => x.PicID).IsRequired();


            
        }
      
    }
}
