using CorporateContacts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateContacts.Domain.Concrete
{
    public class EFDbContextErrorLog : DbContext
    {
        public EFDbContextErrorLog()
        {
            string dbConnction = ConfigurationManager.ConnectionStrings["EFDbContextErrorLogs"].ConnectionString;
            this.Database.Connection.ConnectionString = dbConnction;
            this.Database.CommandTimeout = 1800;
        }

        public DbSet<CCErrorLog> CCErrorLogs { get; set; }
        public DbSet<CCHealthMsgs> CCHealthMsgs { get; set; }
    }
}
