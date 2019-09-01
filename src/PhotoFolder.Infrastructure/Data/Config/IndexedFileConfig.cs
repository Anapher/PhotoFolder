using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotoFolder.Core.Domain.Entities;

namespace PhotoFolder.Infrastructure.Data.Config
{
    public class IndexedFileConfig : IEntityTypeConfiguration<IndexedFile>
    {
        public void Configure(EntityTypeBuilder<IndexedFile> builder)
        {
            builder.HasKey(x => x.Hash);

            builder.Property<string>("_photoPropertiesBitmapHash").HasColumnName("PhotoBitmapHash").UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Property<int?>("_photoPropertiesWidth").HasColumnName("PhotoWidth").UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Property<int?>("_photoPropertiesHeight").HasColumnName("PhotoHeight").UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Metadata.FindNavigation(nameof(IndexedFile.Files))
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
