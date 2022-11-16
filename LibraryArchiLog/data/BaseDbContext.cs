using LibraryArchiLog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;

namespace LibraryArchiLog.data
{
    public class BaseDbContext : DbContext
    {


        public override Task<int> SaveChangesAsync(CancellationToken cancellation = default)
        {
            ChangeCreatedState();
            ChangeDeletedState();
            return base.SaveChangesAsync(cancellation);
        }

        private void ChangeCreatedState()
        {
            var createdEntities = ChangeTracker.Entries().Where(x => x.State == EntityState.Added);
            foreach (var item in createdEntities)
            {
                if (item.Entity is BaseModel model)
                {
                    model.Active = true;
                    model.CreatedAt = DateTime.Now;
                    model.DeletedAt = null;
                }
            }
        }

        private void ChangeDeletedState()
        {
            var deletedEntries = ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted);
            foreach (var item in deletedEntries)
            {
                if (item.Entity is BaseModel model)
                {
                    model.Active = false;
                    model.DeletedAt = DateTime.Now;
                    item.State = EntityState.Modified;
                }
            }
        
    }
        
    }
}
