using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCare.Web.Models.Entities;

namespace ProductCare.Web.Data.Configurations;

public class ServiceCenterConfiguration : IEntityTypeConfiguration<ServiceCenter>
{
    public void Configure(EntityTypeBuilder<ServiceCenter> builder)
    {
        builder.ToTable("ServiceCenters");

        builder.HasKey(sc => sc.Id);

        builder.Property(sc => sc.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(sc => sc.Locality)
            .HasMaxLength(100);

        builder.Property(sc => sc.Address)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(sc => sc.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sc => sc.State)
            .HasMaxLength(50);

        builder.Property(sc => sc.Country)
            .HasMaxLength(50);

        builder.Property(sc => sc.SupportedBrands)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(sc => sc.SupportedCategories)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(sc => sc.Phone)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(sc => sc.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(sc => sc.OpeningHours)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(sc => sc.Website)
            .HasMaxLength(255);

        builder.Property(sc => sc.MapUrl)
            .HasMaxLength(500);

        builder.Property(sc => sc.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(sc => sc.City)
            .HasDatabaseName("IX_ServiceCenters_City");

        builder.HasIndex(sc => sc.IsActive)
            .HasDatabaseName("IX_ServiceCenters_IsActive");
    }
}
