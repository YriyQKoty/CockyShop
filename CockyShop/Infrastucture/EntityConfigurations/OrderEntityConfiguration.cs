using CockyShop.Models.App;
using CockyShop.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CockyShop.Infrastucture.EntityConfigurations
{
    public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasOne<AppUser>()
                .WithMany(a => a.Orders)
                .HasForeignKey(o => o.UserId);

            builder.HasOne<OrderDetails>()
                .WithOne(o => o.Order)
                //.HasForeignKey<OrderDetails>(o => o.OrderId)
                .HasForeignKey<Order>(o => o.OrderDetailsId);
        }
    }
}