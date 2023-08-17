using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.Amount).HasColumnType("decimal(18,4)");
            builder.HasOne(x => x.Product)
                .WithOne(x => x.Discount)
                .HasForeignKey<Discount>(x => x.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
