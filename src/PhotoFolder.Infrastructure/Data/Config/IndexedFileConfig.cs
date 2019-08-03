using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotoFolder.Core;
using PhotoFolder.Core.Domain.Entities;

namespace PhotoFolder.Infrastructure.Data.Config
{
    public class IndexedFileConfig : IEntityTypeConfiguration<IndexedFile>
    {
        public void Configure(EntityTypeBuilder<IndexedFile> builder)
        {
            builder.HasKey(x => x.Hash);

            builder.OwnsOne(x => x.PhotoProperties, a =>
            {
                a.Property(p => p!.BitmapHash)
                    .HasColumnName("PhotoBitmapHash")
                    .HasConversion(x => x.ToString(), x => Hash.Parse(x));
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
