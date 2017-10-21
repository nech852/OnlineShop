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
        private static List<OrderDto> orders = new List<OrderDto>()
        {
            new OrderDto { Id = 1, CustomerName ="John"},
            new OrderDto { Id = 2, CustomerName ="Steve"},
            new OrderDto { Id = 3, CustomerName ="John"},
            new OrderDto { Id = 4, CustomerName ="Mike"},
            new OrderDto { Id = 5, CustomerName ="Peter"},
            new OrderDto { Id = 6, CustomerName ="Ciprian"}
        };

        private static Product[] products = new []
        {   
            new Product{Id = 1,  Price = 4, Name = "Bread"},
            new Product{Id = 2,  Price = 5, Name = "Ham"},
            new Product{Id = 3,  Price = 6, Name = "Milk"},
            new Product{Id = 4,  Price = 7, Name = "Butter"},
            new Product{Id = 5,  Price = 8, Name =  "Potato"},
        };

        private static List<OrderLineDto> orderLineDtos = new List<OrderLineDto>()
        {
            new OrderLineDto{Id = 1, OrderId = 1, ProductId = 1, Quantity = 1},
            new OrderLineDto{Id = 2, OrderId = 1, ProductId = 2, Quantity = 2},
            new OrderLineDto{Id = 3, OrderId = 1, ProductId = 3, Quantity = 3},
            new OrderLineDto{Id = 4, OrderId = 1, ProductId = 4 , Quantity = 5},
            new OrderLineDto{Id = 5, OrderId = 1, ProductId = 5 , Quantity = 5},
            new OrderLineDto{Id = 6, OrderId = 2, ProductId = 1 , Quantity = 2},
        };

        [HttpGet("[action]")]
        public IEnumerable<Order> Search(string mask)
        {
            // if(string.IsNullOrWhiteSpace(mask))
            // {
            //     return Enumerable.Empty<Order>();
            // }
            return orders.Where(order => string.IsNullOrWhiteSpace(mask) ||
                order.CustomerName.IndexOf(mask, StringComparison.OrdinalIgnoreCase) >= 0).
                Select(order => new Order {Id = order.Id, CustomerName = order.CustomerName, 
                TotalPrice = orderLineDtos.Where(oL => oL.OrderId == order.Id).
                Select(oL => oL.Quantity * products.Single(pr => pr.Id == oL.ProductId).Price).Sum()})
                    .AsEnumerable();
        }

        

         private static int orderCounter = 10;

        [HttpPost("[action]")]
        public IEnumerable<Order> AddOrder([FromBody] NewOrder newOrder)
        {
            string customerName = newOrder.CustomerName;
            orders.Add(new OrderDto { Id = orderCounter++, CustomerName = customerName });
            return Search(string.Empty);
        }

//         http.delete('/api/something', new RequestOptions({
//    headers: headers,
//    body: anyObject
// }))

        [HttpDelete("[action]")]
        public IEnumerable<Order> DeleteOrder([FromBody] DeleteOrderArg deleteOrderArg)
        {
            orders.Remove
        }


        [HttpGet("[action]")]
        public IEnumerable<Product> Product()
        {
            return products;
        }

        [HttpGet("[action]")]
        public IEnumerable<OrderLine> OrderLine(int orderId)
        {
            return orderLineDtos.Where(ol => ol.OrderId == orderId).
                       Select(ol => new OrderLine()
                        {
                            Id=ol.Id, OrderId = ol.OrderId, ProductId = ol.ProductId,
                            Quantity = ol.Quantity, 
                            ProductName = products.Single(pr => pr.Id == ol.ProductId).Name,
                            ProductPrice = products.Single(pr => pr.Id == ol.ProductId).Price,
                        });
        }


        private static int productLineCounter = 10;
        
        [HttpPost("[action]")]
        public IEnumerable<OrderLine> AddProductLine([FromBody] NewOrderLine newOrderLine)
        {           
            OrderDto order = orders.Single(ord => ord.Id == newOrderLine.OrderId);
            Product product = products.Single(prod => prod.Id == newOrderLine.ProductId);
            var orderLineDto = orderLineDtos.
                SingleOrDefault(oL => oL.OrderId == newOrderLine.OrderId && oL.ProductId == newOrderLine.ProductId);
            if(orderLineDto == null)
            {
                orderLineDto = 
                    new OrderLineDto {Id = productLineCounter++, OrderId = newOrderLine.OrderId,
                     ProductId = newOrderLine.ProductId};
                 orderLineDtos.Add(orderLineDto);
            }

            orderLineDto.Quantity += newOrderLine.Quantity;
            return OrderLine(newOrderLine.OrderId);
        }

    }

    
}
