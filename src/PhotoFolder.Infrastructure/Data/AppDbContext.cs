#pragma warning disable CS8618 // Non-nullable field is uninitialized. Fields are automatically initialized by EF Core

using PhotoFolder.Core.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using PhotoFolder.Infrastructure.Data.Config;
using PhotoFolder.Core.Domain.Entities;

namespace PhotoFolder.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<IndexedFile> IndexedFiles { get; set; }
        public DbSet<FileOperation> FileOperations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new IndexedFileConfig());
            builder.ApplyConfiguration(new FileLocationConfig());
            builder.ApplyConfiguration(new FileOperationConfig());
        }

        public override int SaveChanges()
        {
            AddAuditInfo();
            return base.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            AddAuditInfo();
            return await base.SaveChangesAsync();
        }

        private void AddAuditInfo()
        {
            var entries = ChangeTracker.Entries()
                .Where(x => x.Entity is IEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    ((IEntity) entry.Entity).CreatedOn = DateTimeOffset.UtcNow;
                }
                ((IEntity) entry.Entity).ModifiedOn = DateTimeOffset.UtcNow;
            }
        }
    }
}
