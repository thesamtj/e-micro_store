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
            logger.LogInformation($"Data Seed method called 03: OrderContext");
            logger.LogInformation($"Checking if Order Table exists: {orderContext.Orders.Any()}.");
            if (!orderContext.Orders.Any())
            {
                logger.LogInformation($"Creating Order Table and adding seed data: {typeof(OrderContext).Name}.");
                orderContext.Orders.AddRange(GetOrders());
                logger.LogInformation($"Saving changes to the DB: {typeof(OrderContext).Name}.");
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
