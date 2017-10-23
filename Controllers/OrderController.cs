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
        private OrderContext _orderContext;

        public OrderController(OrderContext orderContext)
        {
            this._orderContext = orderContext;
        }

        [HttpGet("Orders")]
        public async Task<IEnumerable<Order>> GetOrders(string mask)
        {
            IQueryable<Order> allOrders = _orderContext.Orders.Select(order => new Order {Id = order.Id,
                CustomerName = order.CustomerName, 
                TotalPrice = order.OrderLines.Select(line => line.Quantity * line.Product.Price).Sum()});
            IQueryable<Order> result = allOrders;
            if(!string.IsNullOrWhiteSpace(mask)){
                result = allOrders.Where(order => EF.Functions.Like(order.CustomerName, $"%{mask.Trim()}%"));
            }
            return await result.ToListAsync();
        }

        [HttpPost("[action]")]
        public async Task<IEnumerable<Order>> AddOrder([FromBody] NewOrderArgs args)
        {
            _orderContext.Orders.Add(new OrderDto { CustomerName = args.CustomerName});
            await _orderContext.SaveChangesAsync();
            return await GetOrders(args.Mask);
        }


        [HttpDelete("[action]")]
        public async Task<IEnumerable<Order>> DeleteOrder([FromBody] DeleteOrderArgs args)
        {
            // OrderDto orderDto = new OrderDto { Id = args.OrderId };
            // _orderContext.Entry(orderDto).State = EntityState.Deleted;
            OrderDto orderDto = _orderContext.Orders.Include(ord => ord.OrderLines).Single(ord => ord.Id == args.OrderId);
            _orderContext.RemoveRange(orderDto.OrderLines);
            _orderContext.Remove(orderDto);
            await _orderContext.SaveChangesAsync();
            return await GetOrders(args.Mask);
        }
        
        [HttpDelete("[action]")]
        public async Task<IEnumerable<OrderLine>> DeleteOrderLine([FromBody] DeleteOrderLineArgs args)
        {
            OrderLineDto orderLine = new OrderLineDto { Id = args.OrderLineId };
            _orderContext.Entry(orderLine).State = EntityState.Deleted;
            //TODO: handle situations when order or order line is not found
            await _orderContext.SaveChangesAsync();
            return await GetOrderLines(args.OrderId);
        }


        [HttpGet("Products")]
        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var result = await _orderContext.Products.ToListAsync();
            return result;
        }

        [HttpGet("OrderLines")]
        public async Task<IEnumerable<OrderLine>> GetOrderLines(int orderId)
        {
            return await _orderContext.OrderLines.Where(ol => ol.OrderId == orderId).
                       Select(ol => new OrderLine()
                        {
                            Id=ol.Id, OrderId = ol.OrderId, ProductId = ol.ProductId,
                            Quantity = ol.Quantity, 
                            ProductName = ol.Product.Name,
                            ProductPrice = ol.Product.Price,
                        }).ToListAsync();
        }
        
        [HttpPost("[action]")]
        public async Task<IEnumerable<OrderLine>> AddOrderLine([FromBody] NewOrderLineArgs args)
        {       
            OrderLineDto orderLine = _orderContext.OrderLines.SingleOrDefault(line => line.OrderId == args.OrderId
                && line.ProductId == args.ProductId);
             if(orderLine == null) 
             {
                orderLine = 
                    new OrderLineDto { OrderId = args.OrderId, ProductId = args.ProductId };
                _orderContext.OrderLines.Add(orderLine);    
             }

             orderLine.Quantity += args.Quantity;

             await _orderContext.SaveChangesAsync();

             return await GetOrderLines(args.OrderId);
        }

    }

    
}
