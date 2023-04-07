using AuthenticationWithIdentity.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationWithIdentity.Entities
{
    public class DemoContext : IdentityDbContext<User>
    {
        public DemoContext(DbContextOptions options)
        : base(options)    
        { 
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new RoleConfiguration());
        }

        public DbSet<User> Users { get; set; }
    }
}
