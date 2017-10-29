using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace OnlineShop.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(OrderContext orderContext)
        {
            if(!orderContext.Products.Any())
            {
                var products = new ProductDto[]
                {
                    new ProductDto {Name = "Milk", Price = 5.5M},
                    new ProductDto {Name = "Bread", Price = 6M},
                    new ProductDto {Name = "Chease", Price = 3.3M},
                    new ProductDto {Name = "Potato", Price = 8.8M},
                    new ProductDto {Name = "Orange", Price = 2.2M}
                };
                orderContext.Products.AddRange(products);
                
            }
            await orderContext.SaveChangesAsync();
        }
    }
}