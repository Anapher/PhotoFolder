using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotoFolder.Core;
using PhotoFolder.Core.Domain.Entities;

namespace PhotoFolder.Infrastructure.Data.Config
{
    public class ListedFileConfig : IEntityTypeConfiguration<IndexedFile>
    {
        public void Configure(EntityTypeBuilder<IndexedFile> builder)
        {
            builder.Property(x => x.FileHash).HasConversion(x => x.ToString(), x => Hash.Parse(x));
            builder.HasIndex(x => x.Filename).IsUnique();
        }
    }
}
