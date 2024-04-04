using Basket.Core.Entities;
using Basket.Core.Repositories;
using Basket.Infrastructure.Data;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basket.Infrastructure.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly BasketContext _context;

        public BasketRepository(BasketContext context)
        {
            _context = context;
        }

        public async Task<ShoppingCart> GetBasket(string userName)
        {
            return await _context.ShoppingCarts.FindAsync(userName);
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart shoppingCart)
        {
            await _context.ShoppingCarts.AddAsync(shoppingCart);
            await _context.SaveChangesAsync();
            return await GetBasket(shoppingCart.UserName);
        }

        public async Task DeleteBasket(string userName)
        {
            var basket = await GetBasket(userName);
            if (basket == null)
            {
                //return NotFound();
            }

            _context.ShoppingCarts.Remove(basket);
            await _context.SaveChangesAsync();
        }
    }
}
