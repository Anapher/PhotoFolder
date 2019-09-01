using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotoFolder.Core.Domain.Entities;

namespace PhotoFolder.Infrastructure.Data.Config
{
    public class FileLocationConfig : IEntityTypeConfiguration<FileLocation>
    {
        public void Configure(EntityTypeBuilder<FileLocation> builder)
        {
            builder.HasKey(x => x.RelativeFilename);
            builder.Property(x => x.RelativeFilename).IsRequired();
        }
    }
}
