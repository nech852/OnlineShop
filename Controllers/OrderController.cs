using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;

namespace OnlineShop.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        // private static string[] Orders = new[]
        // {
        //     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        // };

        private static Order[] Orders = new[]
        {
            new Order{ Id = 1, CustomerName ="John", TotalPrice = 25},
            new Order{ Id = 2, CustomerName ="Steve", TotalPrice = 37},
            new Order{ Id = 3, CustomerName ="John", TotalPrice = 25},
            new Order{ Id = 4, CustomerName ="Mike", TotalPrice = 37},
            new Order{ Id = 5, CustomerName ="Peter", TotalPrice = 25},
            new Order{ Id = 6, CustomerName ="Ciprian", TotalPrice = 37}
        };

        [HttpGet("[action]")]
        public IEnumerable<Order> Search(string mask)
        {
            if(string.IsNullOrWhiteSpace(mask))
            {
                return Orders;
            }
            return Orders.Where(order => 
                order.CustomerName.IndexOf(mask, StringComparison.OrdinalIgnoreCase) >= 0)
                    .AsEnumerable();
        }

    }

    
}
