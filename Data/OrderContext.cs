using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Data
{
    public class OrderContext : DbContext
    {        
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
        }

        public DbSet<OrderDto> Orders { get; set; }
        public DbSet<OrderLineDto> OrderLines { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDto>().ToTable("Order");
            modelBuilder.Entity<OrderLineDto>().ToTable("OrderLine");
            modelBuilder.Entity<Product>().ToTable("Product");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }

}