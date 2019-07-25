using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotoFolder.Core.Domain.Entities;

namespace PhotoFolder.Infrastructure.Data.Config
{
    public class FileInformationConfig : IEntityTypeConfiguration<IndexedFile>
    {
        public void Configure(EntityTypeBuilder<IndexedFile> builder)
        {
            builder.HasKey(x => x.Hash);

            builder.OwnsOne(x => x.PhotoProperties, a =>
            {
                a.Property(p => p!.BitmapHash)
                    .HasColumnName("PhotoBitmapHash");
                a.Property(p => p!.Height)
                    .HasColumnName("PhotoHeight");
                a.Property(p => p!.Width)
                   .HasColumnName("PhotoWidth");
            });

            builder.Metadata.FindNavigation(nameof(IndexedFile.Files))
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
