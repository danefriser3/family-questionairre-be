using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class FamilyContext(DbContextOptions<FamilyContext> options) : DbContext(options)
    {
        public DbSet<Child> Children { get; set; }
    }
}
