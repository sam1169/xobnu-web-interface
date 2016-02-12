using Xobnu.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xobnu.Domain.Concrete
{
    public class EFDbContextLogonLog : DbContext
    {
        public EFDbContextLogonLog()
        {
            string dbConnction = ConfigurationManager.ConnectionStrings["EFDbContextLogonLog"].ConnectionString;
            this.Database.Connection.ConnectionString = dbConnction;
        }

        public DbSet<CCLogonLog> CCLogonLogs { get; set; }
    }
}
