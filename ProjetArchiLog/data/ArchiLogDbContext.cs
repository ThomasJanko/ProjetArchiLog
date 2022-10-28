using LibraryArchiLog.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProjetArchiLog.Models;
using System;
using LibraryArchiLog.data;
using ProjetArchiLog.Models;
using System.Runtime.ConstrainedExecution;

namespace Projet1ArchiLog.data
{
    public class ArchiLogDbContext : BaseDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            base.OnConfiguring(options);
            options.UseSqlServer(@"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = database; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False");
        }

        public DbSet<Product> Products { get; set; }
        

    }
}
