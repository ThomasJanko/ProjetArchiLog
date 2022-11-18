using LibraryTests.Mock.Models;
using Microsoft.EntityFrameworkCore;
using ProjetArchiLog.data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryTests.Mock
{
    public class MockDbContext : ArchiLogDbContext
    {
        public MockDbContext(DbContextOptions options)
        {
        }

        public static MockDbContext GetDbContext(bool withData = true)
        {
            var options = new DbContextOptionsBuilder().UseInMemoryDatabase("dbtest").Options;
            var db = new MockDbContext(options);

            if (withData)
            {
                string dateTime = "2019-09-09T00:00:00";
                DateTime dt = DateTime.ParseExact(dateTime, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                db.Products.Add(new ProductMock { ID = 1, Name = "Pizza 1", Category = "pizza", Price = 10,  CreatedAt = dt, Active = true });
                db.Products.Add(new ProductMock { ID = 2, Name = "Pizza 2", Category = "pizza", Price = 10, CreatedAt = dt, Active = true });
                db.Products.Add(new ProductMock { ID = 3, Name = "Pizza 3", Category = "pizza", Price = 10, CreatedAt = dt, Active = true });
                db.Products.Add(new ProductMock { ID = 4, Name = "Pizza 4", Category = "pizza", Price = 10, CreatedAt = dt, Active = true });
                db.Products.Add(new ProductMock { ID = 5, Name = "Pizza 5", Category = "pizza", Price = 10, CreatedAt = dt, Active = true });
                db.Products.Add(new ProductMock { ID = 6, Name = "Pizza 6", Category = "pizza", Price = 10, CreatedAt = dt, Active = true });

                db.SaveChanges();
            }

            return db;
        }
    }
}

