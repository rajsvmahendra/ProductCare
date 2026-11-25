using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCare.Web.Models.Entities;

namespace ProductCare.Web.Data.Configurations;

public class NetworkQualityReportConfiguration : IEntityTypeConfiguration<NetworkQualityReport>
{
    public void Configure(EntityTypeBuilder<NetworkQualityReport> builder)
    {
        builder.ToTable("NetworkQualityReports");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.CampusZone)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(r => r.NetworkType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(r => r.CampusZone)
            .HasDatabaseName("IX_NetworkQualityReports_CampusZone");

        builder.HasIndex(r => r.MeasuredAtUtc)
            .HasDatabaseName("IX_NetworkQualityReports_MeasuredAtUtc");

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
