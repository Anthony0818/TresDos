using Microsoft.EntityFrameworkCore;
using TresDos.Core.Entities;

namespace TresDos.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<tb_TwoD> tb_TwoD { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
