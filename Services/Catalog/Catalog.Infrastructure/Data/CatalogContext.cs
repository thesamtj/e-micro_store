using Catalog.Core.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Data
{
    public class CatalogContext : ICatalogContext
    {
        public IMongoCollection<Product> Products { get; }
        public IMongoCollection<ProductBrand> Brands { get; }
        public IMongoCollection<ProductType> Types { get; }

        public CatalogContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetSection("DatabaseSettings").GetConnectionString("ConnectionString"));
            var database = client.GetDatabase(configuration.GetSection("DatabaseSettings").GetConnectionString("DatabaseName"));
            Brands = database.GetCollection<ProductBrand>(
                configuration.GetSection("DatabaseSettings").GetConnectionString("BrandsCollection"));
            Types = database.GetCollection<ProductType>(
                configuration.GetSection("DatabaseSettings").GetConnectionString("TypesCollection"));
            Products = database.GetCollection<Product>(
                configuration.GetSection("DatabaseSettings").GetConnectionString("CollectionName"));

            BrandContextSeed.SeedData(Brands);
            TypeContextSeed.SeedData(Types);
            CatalogContextSeed.SeedData(Products);
        }
    }
}
