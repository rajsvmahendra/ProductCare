using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCare.Web.Models.Entities;

namespace ProductCare.Web.Data.Configurations;

public class ServiceRequestHistoryConfiguration : IEntityTypeConfiguration<ServiceRequestHistory>
{
    public void Configure(EntityTypeBuilder<ServiceRequestHistory> builder)
    {
        builder.ToTable("ServiceRequestHistories");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.ChangedByUserName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(h => h.Remarks)
            .HasMaxLength(1000);

        builder.HasIndex(h => h.ServiceRequestId)
            .HasDatabaseName("IX_ServiceRequestHistories_ServiceRequestId");

        builder.HasIndex(h => h.CreatedAtUtc)
            .HasDatabaseName("IX_ServiceRequestHistories_CreatedAtUtc");

        // Relationships
        builder.HasOne(h => h.ServiceRequest)
            .WithMany(sr => sr.StatusHistories)
            .HasForeignKey(h => h.ServiceRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.ChangedByUser)
            .WithMany(u => u.ServiceRequestHistories)
            .HasForeignKey(h => h.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
