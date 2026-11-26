using Microsoft.AspNetCore.Identity;
using ProductCare.Web.Models.Entities;

namespace ProductCare.Web.Services;

public interface IDbInitializerService
{
    Task EnsureRolesExistAsync();

    Task<string> ProvisionAdminUserAsync(string? email = null, string? password = null);

    Task EnsureDatabaseObjectsCreatedAsync();

    Task SeedDemoDataAsync();
}
