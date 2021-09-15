using CockyShop.Infrastucture.EntityConfigurations;
using CockyShop.Models.App;
using CockyShop.Models.Enums;
using CockyShop.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CockyShop.Infrastucture
{
    public class AppDbContext : IdentityDbContext<AppUser,AppRole, string>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductStock> ProductStocks { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        
        public DbSet<OrderedProduct> OrderedProducts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrdersDetails { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        
        public DbSet<City> Cities { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductEntityConfiguration).Assembly);
        }
    }
}