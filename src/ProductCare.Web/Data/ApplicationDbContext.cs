using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductCare.Web.Models.Entities;

namespace ProductCare.Web.Data;

/// <summary>
/// Entity Framework Core database context for ProductCare.
/// Integrates ASP.NET Core Identity with application domain models.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
    public DbSet<ServiceRequestHistory> ServiceRequestHistories => Set<ServiceRequestHistory>();

    // Keyless entities for SQL Server View and Stored Procedure projections
    public DbSet<ProductWarrantySummaryView> ProductWarrantySummaries => Set<ProductWarrantySummaryView>();
    public DbSet<ServiceTicketStatisticsDto> ServiceTicketStatistics => Set<ServiceTicketStatisticsDto>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all entity configurations in this assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Configure SQL Server View mapping (Keyless entity)
        builder.Entity<ProductWarrantySummaryView>(v =>
        {
            v.HasNoKey();
            v.ToView("vw_ProductWarrantySummary");
        });

        // Configure Stored Procedure Result mapping (Keyless entity)
        builder.Entity<ServiceTicketStatisticsDto>(s =>
        {
            s.HasNoKey();
        });
    }
}
