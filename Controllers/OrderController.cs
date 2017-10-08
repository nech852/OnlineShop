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
        private static Order[] orders = new[]
        {
            new Order{ Id = 1, CustomerName ="John", TotalPrice = 25},
            new Order{ Id = 2, CustomerName ="Steve", TotalPrice = 37},
            new Order{ Id = 3, CustomerName ="John", TotalPrice = 25},
            new Order{ Id = 4, CustomerName ="Mike", TotalPrice = 37},
            new Order{ Id = 5, CustomerName ="Peter", TotalPrice = 25},
            new Order{ Id = 6, CustomerName ="Ciprian", TotalPrice = 37}
        };

        private static Product[] products = new []
        {   
            new Product{Id = 1,  Price = 4, Name = "Bread"},
            new Product{Id = 2,  Price = 5, Name = "Ham"},
            new Product{Id = 3,  Price = 6, Name = "Milk"},
            new Product{Id = 4,  Price = 7, Name = "Butter"},
            new Product{Id = 5,  Price = 8, Name =  "Potato"},
        };

        [HttpGet("[action]")]
        public IEnumerable<Order> Search(string mask)
        {
            if(string.IsNullOrWhiteSpace(mask))
            {
                return orders;
            }
            return orders.Where(order => 
                order.CustomerName.IndexOf(mask, StringComparison.OrdinalIgnoreCase) >= 0)
                    .AsEnumerable();
        }

        [HttpGet("[action]")]
        public IEnumerable<Product> Product()
        {

            return products;
        }

    
    }

    
}
