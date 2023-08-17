using Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class ShoppingCartItemConfiguration : IEntityTypeConfiguration<ShoppingCartItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
        {
            builder.Property(x => x.Quantity).IsRequired();
            builder.HasOne(x => x.ShoppingCart)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.ShoppingCartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Product)
            .WithOne(x => x.ShoppingCartItem)
            .HasForeignKey<ShoppingCartItem>(x => x.ProductId);
        }
    }

    public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
     
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,4)");
            builder.Property(x => x.TotalAmount).IsRequired();
            //builder.HasMany(x => x.Items)
            //    .WithOne()
            //    .HasForeignKey(x => x.ShoppingCartId)
            //    .OnDelete(DeleteBehavior.Cascade);

           
        }
    }
}
