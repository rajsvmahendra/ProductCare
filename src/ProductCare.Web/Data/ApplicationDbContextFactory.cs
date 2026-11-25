using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProductCare.Web.Data;

/// <summary>
/// Design-time factory enabling EF Core CLI tooling (migrations, script generation)
/// to construct ApplicationDbContext independently of runtime web host startup.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Default SQL Server LocalDB connection string per project specification
        const string connectionString = "Server=(localdb)\\ProductCareTest;Database=ProductCareDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
