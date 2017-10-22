using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        //TODO: add underscore
        private OrderContext orderContext;

        public OrderController(OrderContext orderContext)
        {
            this.orderContext = orderContext;
        }

        [HttpGet("[action]")]
        public IEnumerable<Order> Search(string mask)
        {
            IQueryable<Order> allOrders = orderContext.Orders.Select(order => new Order {Id = order.Id,
                CustomerName = order.CustomerName, 
                TotalPrice = order.OrderLines.Select(line => line.Quantity * line.Product.Price).Sum()});
            IQueryable<Order> result = allOrders;
            if(!string.IsNullOrWhiteSpace(mask)){
                result = allOrders.Where(order => EF.Functions.Like(order.CustomerName, mask.Trim()));
            }
            return result.ToList();
        }

        [HttpPost("[action]")]
        public IEnumerable<Order> AddOrder([FromBody] NewOrder newOrder)
        {
            string customerName = newOrder.CustomerName;
            orderContext.Orders.Add(new OrderDto { CustomerName = customerName });
            orderContext.SaveChanges();
            //TODO: search by mask; specify current mask on UI
            return Search(string.Empty);
        }


        //Rename "DeleteOrderArg deleteOrderArg" to "DeleteOrderArgs args"
        [HttpDelete("[action]")]
        public IEnumerable<Order> DeleteOrder([FromBody] DeleteOrderArg deleteOrderArg)
        {
            OrderDto orderDto = new OrderDto { Id = deleteOrderArg.OrderId };
            orderContext.Entry(orderDto).State = EntityState.Deleted;
            orderContext.SaveChanges();
            //orderContext.Orders.RemoveRange(ord => ord.Id == deleteOrderArg.OrderId);
            return Search(deleteOrderArg.Mask);
        }
        
        [HttpDelete("[action]")]
        public IEnumerable<OrderLine> DeleteOrderLine([FromBody] DeleteOrderLineArgs args)
        {
            OrderLineDto orderLine = new OrderLineDto { Id = args.OrderLineId };
            orderContext.Entry(orderLine).State = EntityState.Deleted;
            //TODO: handle situations when order or order line is not found
            //orderContext.OrderLines.RemoveRange(ordLine => ordLine.Id == args.OrderLineId);
            orderContext.SaveChanges();
            return OrderLine(args.OrderId);
        }


        [HttpGet("[action]")]
        public IEnumerable<Product> Product()
        {
            //return new List<Product>();
            var result = orderContext.Products.ToList();
            return result;
        }

        [HttpGet("[action]")]
        public IEnumerable<OrderLine> OrderLine(int orderId)
        {
            return orderContext.OrderLines.Where(ol => ol.OrderId == orderId).
                       Select(ol => new OrderLine()
                        {
                            Id=ol.Id, OrderId = ol.OrderId, ProductId = ol.ProductId,
                            Quantity = ol.Quantity, 
                            ProductName = ol.Product.Name,
                            ProductPrice = ol.Product.Price,
                        }).ToList();
        }
        
        //TODO: Rename to AddOrderLine
        [HttpPost("[action]")]
        public IEnumerable<OrderLine> AddProductLine([FromBody] NewOrderLine newOrderLine)
        {       
            OrderLineDto orderLine = orderContext.OrderLines.SingleOrDefault(line => line.OrderId == newOrderLine.OrderId
             && line.ProductId == newOrderLine.ProductId);
             if(orderLine == null) 
             {
                orderLine = 
                    new OrderLineDto {OrderId = newOrderLine.OrderId, ProductId = newOrderLine.ProductId};
                orderContext.OrderLines.Add(orderLine);    
             }

             orderLine.Quantity += newOrderLine.Quantity;

             orderContext.SaveChanges();

             return OrderLine(newOrderLine.OrderId);
        }

    }

    
}
