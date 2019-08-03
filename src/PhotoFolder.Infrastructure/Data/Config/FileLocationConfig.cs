using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotoFolder.Core.Domain.Entities;

namespace PhotoFolder.Infrastructure.Data.Config
{
    public class FileLocationCinfig : IEntityTypeConfiguration<FileLocation>
    {
        public void Configure(EntityTypeBuilder<FileLocation> builder)
        {
            builder.HasKey(x => x.Filename);
            builder.Property(x => x.Filename).IsRequired();
        }
    }
}
