using Microsoft.EntityFrameworkCore;

namespace PhotoFolder.Infrastructure.Data
{
    public class AppDbContextOptionsBuilder : IAppDbContextOptionsBuilder
    {
        public DbContextOptions<AppDbContext> Build(string filename)
        {
            var dbContextBuilder = new DbContextOptionsBuilder<AppDbContext>();
            dbContextBuilder.UseSqlite($"Data Source={filename};");

            return dbContextBuilder.Options;
        }
    }

    public interface IAppDbContextOptionsBuilder
    {
        DbContextOptions<AppDbContext> Build(string filename);
    }
}
