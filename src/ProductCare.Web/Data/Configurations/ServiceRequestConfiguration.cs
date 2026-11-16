using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCare.Web.Models.Entities;

namespace ProductCare.Web.Data.Configurations;

public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
{
    public void Configure(EntityTypeBuilder<ServiceRequest> builder)
    {
        builder.ToTable("ServiceRequests");

        builder.HasKey(sr => sr.Id);

        builder.Property(sr => sr.TicketNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(sr => sr.Subject)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(sr => sr.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(sr => sr.AssignedTechnician)
            .HasMaxLength(100);

        builder.Property(sr => sr.AdminRemarks)
            .HasMaxLength(2000);

        // Unique index on TicketNumber
        builder.HasIndex(sr => sr.TicketNumber)
            .IsUnique()
            .HasDatabaseName("UX_ServiceRequests_TicketNumber");

        // Operational lookup indexes
        builder.HasIndex(sr => sr.UserId)
            .HasDatabaseName("IX_ServiceRequests_UserId");

        builder.HasIndex(sr => sr.ProductId)
            .HasDatabaseName("IX_ServiceRequests_ProductId");

        builder.HasIndex(sr => sr.Status)
            .HasDatabaseName("IX_ServiceRequests_Status");

        builder.HasIndex(sr => sr.CreatedAtUtc)
            .HasDatabaseName("IX_ServiceRequests_CreatedAtUtc");

        // Relationships
        builder.HasOne(sr => sr.Product)
            .WithMany(p => p.ServiceRequests)
            .HasForeignKey(sr => sr.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sr => sr.User)
            .WithMany(u => u.ServiceRequests)
            .HasForeignKey(sr => sr.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
