using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PhotoFolder.Infrastructure.Shared
{
    public abstract class DesignTimeDbContextFactoryBase<TContext> : IDesignTimeDbContextFactory<TContext> where TContext : DbContext
    {
        public TContext CreateDbContext(string[] args)
        {
            return Create("DataSource=:memory:");
        }

        protected abstract TContext CreateNewInstance(DbContextOptions<TContext> options);

        private TContext Create(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException(
             $"{nameof(connectionString)} is null or empty.",
             nameof(connectionString));

            var optionsBuilder = new DbContextOptionsBuilder<TContext>();

            Console.WriteLine("DesignTimeDbContextFactory.Create(string): Connection string: {0}", connectionString);

            optionsBuilder.UseSqlite(connectionString);

            var options = optionsBuilder.Options;
            return CreateNewInstance(options);
        }
    }
}