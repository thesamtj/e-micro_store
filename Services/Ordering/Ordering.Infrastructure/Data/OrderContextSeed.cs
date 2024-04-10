using Microsoft.Extensions.Logging;
using Ordering.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Data
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
        {
            if (!orderContext.Orders.Any())
            {
                orderContext.Orders.AddRange(GetOrders());
                await orderContext.SaveChangesAsync();
                logger.LogInformation($"Ordering Database: {typeof(OrderContext).Name} seeded.");
            }
        }

        private static IEnumerable<Order> GetOrders()
        {
            return new List<Order>
            {
                new()
                {
                    UserName = "sam",
                    FirstName = "Samuel",
                    LastName = "Tijani",
                    EmailAddress = "omalitijanisam@gmail.com",
                    AddressLine = "Abuja",
                    Country = "Nigeria",
                    TotalPrice = 750,
                    State = "AB",
                    ZipCode = "560001",

                    CardName = "Visa",
                    CardNumber = "1234567890123456",
                    CreatedBy = "Samuel",
                    Expiration = "12/25",
                    Cvv = "123",
                    PaymentMethod = 1,
                    LastModifiedBy = "Samuel",
                    LastModifiedDate = new DateTime(),
                }
            };
        }
    }
}
