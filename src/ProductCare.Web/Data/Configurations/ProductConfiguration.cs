using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCare.Web.Models.Entities;

namespace ProductCare.Web.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Brand)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.ModelNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.SerialNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Category)
            .IsRequired();

        builder.Property(p => p.SubCategory)
            .HasMaxLength(100);

        builder.Property(p => p.PurchaseStore)
            .HasMaxLength(150);

        builder.Property(p => p.PurchasePrice)
            .HasPrecision(18, 2);

        builder.Property(p => p.InvoiceFileName)
            .HasMaxLength(255);

        builder.Property(p => p.InvoiceOriginalFileName)
            .HasMaxLength(255);

        builder.Property(p => p.InvoiceContentType)
            .HasMaxLength(100);

        builder.Property(p => p.Notes)
            .HasMaxLength(1000);

        builder.Property(p => p.IsArchived)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.ArchivedAtUtc);

        // Indexes for performance
        builder.HasIndex(p => new { p.UserId, p.IsArchived })
            .HasDatabaseName("IX_Products_UserId_IsArchived");

        builder.HasIndex(p => p.UserId)
            .HasDatabaseName("IX_Products_UserId");

        builder.HasIndex(p => p.WarrantyExpiryDate)
            .HasDatabaseName("IX_Products_WarrantyExpiryDate");

        // Unique constraint: A customer cannot register the same serial number twice
        builder.HasIndex(p => new { p.UserId, p.SerialNumber })
            .IsUnique()
            .HasDatabaseName("UX_Products_UserId_SerialNumber");

        // Relationship with User
        builder.HasOne(p => p.User)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
