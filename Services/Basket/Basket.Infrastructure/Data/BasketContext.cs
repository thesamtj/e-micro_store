using Basket.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basket.Infrastructure.Data
{
    public class BasketContext : DbContext
    {
        public BasketContext(DbContextOptions<BasketContext> options) : base(options)
        {
        }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configure Primary Key using HasKey method
            //modelBuilder.Entity<ShoppingCart>(entity =>
            //{
            //    entity.HasKey(e => e.UserName);
            //});
            //modelBuilder.Entity<ShoppingCartItem>(entity =>
            //{
            //    entity.HasKey(e => e.ProductId);

            //    //entity.HasOne(d => d.ShoppingCart)
            //    //.WithMany(p => p.Items)
            //    //.HasForeignKey(d => d.ShoppingCartUserName)
            //    //.OnDelete(DeleteBehavior.ClientCascade);
            //});
            modelBuilder.Entity<ShoppingCart>().HasKey(s => s.UserName);
            modelBuilder.Entity<ShoppingCartItem>().HasKey(s => s.ProductId);
        }
    }
}
