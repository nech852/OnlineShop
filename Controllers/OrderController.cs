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

        private static OrderLine[] orderLines = new []
        {
            new OrderLine{Id = 1, OrderId = 1, ProductId = 1, ProductPrice = 4.5f, Quantity = 6, ProductName = "Bread" },
            new OrderLine{Id = 2, OrderId = 1, ProductId = 1, ProductPrice = 4.5f, Quantity = 6, ProductName = "Ham" },
            new OrderLine{Id = 3, OrderId = 1, ProductId = 1, ProductPrice = 4.5f, Quantity = 6, ProductName = "Potato" },
            new OrderLine{Id = 4, OrderId = 1, ProductId = 1, ProductPrice = 4.5f, Quantity = 6, ProductName = "Tomato" },
            new OrderLine{Id = 5, OrderId = 1, ProductId = 1, ProductPrice = 4.5f, Quantity = 6, ProductName = "Onion" },
            new OrderLine{Id = 6, OrderId = 1, ProductId = 1, ProductPrice = 4.5f, Quantity = 6, ProductName = "Garlic" },
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

        [HttpGet("[action]")]
        public IEnumerable<OrderLine> OrderLine(int orderId)
        {
            return orderLines;
        }

        [HttpPost]
        public IEnumerable AddProduct([FromBody] int orderId, [FromBody] int productId, [FromBody] int quantity)
        {
            Order order = orders.Single(ord => ord.Id == orderId);
            Product product = products.Single(prod => prod.Id == productId);
            var orderLine = 
                new OrderLine { 
                    Id = 6, OrderId = orderId, ProductId = productId,
                    ProductPrice = product.Price, Quantity = quantity, ProductName = product.Name 
                 };
            orderLines.RemoveAll
        }

    }

    
}
