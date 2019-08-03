using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotoFolder.Core.Domain.Entities;

namespace PhotoFolder.Infrastructure.Data.Config
{
    public class FileOperationConfig : IEntityTypeConfiguration<FileOperation>
    {
        public void Configure(EntityTypeBuilder<FileOperation> builder)
        {
            builder.Ignore(x => x.SourceFile);
            builder.Ignore(x => x.TargetFile);
        }
    }
}
