using LibraryArchiLog.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProjetArchiLog.Models;
using System;
using LibraryArchiLog.data;


namespace ProjetArchiLog.data
{
    public class ArchiLogDbContext : BaseDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            base.OnConfiguring(options);
            options.UseSqlServer(@"Server=tcp:archilogdjib.database.windows.net,1433;Initial Catalog=archilogdb;Persist Security Info=False;User ID=archilogadmin;Password=E$s@p3dtom;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            //options.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        }

        public DbSet<Product> Products { get; set; }


    }
}
