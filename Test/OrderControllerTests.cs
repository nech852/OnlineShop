using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using OnlineShop.Controllers;


namespace Test
{
    [TestClass]
    public class OrderControllerTests
    {
        [TestMethod]
       public async Task TestMethod1()
       // public void TestMethod1()
        {
           // var migrator = new DbMigrator(configuration);
          //  migrator.Update();
            //OrderContextModelSnapshot

            IConfiguration configuration = GetConfig();

            string connectionString = configuration.GetConnectionString("DefaultConnection");
            
            var optionsBuilder = new DbContextOptionsBuilder<OrderContext>();
            optionsBuilder.UseSqlServer(connectionString);
            DbContextOptions<OrderContext> contextOptions = optionsBuilder.Options;
            
            var orderContext = new OrderContext(contextOptions);
            var orderController = new OrderController(orderContext);

            await orderContext.Database.MigrateAsync();
            IEnumerable<Order> orders = await orderController.GetOrders(string.Empty);
            await orderContext.Database.EnsureDeletedAsync();
        }

        private static IConfiguration GetConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json");

            return builder.Build();
        }
    }
    
}
