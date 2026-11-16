using ProductCare.Web.Models.Entities;

namespace ProductCare.Web.Services;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}
