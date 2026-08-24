using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class IdentityProviderContext(DbContextOptions<IdentityProviderContext> options) : IdentityDbContext<SchurkoPortfolio.Data.ApplicationUser>(options)
{
}
