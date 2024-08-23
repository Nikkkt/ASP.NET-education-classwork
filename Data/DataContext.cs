using Microsoft.EntityFrameworkCore;

namespace ASP.NET_Classwork.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Entities.User> Users { get; set; }
        public DbSet<Entities.Token> Tokens { get; set; }
        public DbSet<Entities.Product> Products { get; set; }
        public DbSet<Entities.ProductGroup> Groups { get; set; }
        public DataContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Entities.Product>()
                .HasOne(p => p.Group)
                .WithMany(g => g.Products)
                .HasForeignKey(p => p.GroupId)
                .HasPrincipalKey(g => g.Id);
        }
    }
}
