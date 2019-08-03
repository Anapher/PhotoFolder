using Microsoft.EntityFrameworkCore;
using PhotoFolder.Infrastructure.Shared;

namespace PhotoFolder.Infrastructure.Data
{
    public class AppDbContextFactory : DesignTimeDbContextFactoryBase<AppDbContext>
    {
        protected override AppDbContext CreateNewInstance(DbContextOptions<AppDbContext> options)
        {
            return new AppDbContext(options);
        }
    }
}
