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
        public async Task<List<Order>> GetOrders(string mask = null)
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
        public async Task<List<Order>> AddOrder([FromBody] NewOrderArgs args)
        {
            _orderContext.Orders.Add(new OrderDto { CustomerName = args.CustomerName});
            await _orderContext.SaveChangesAsync();
            return await GetOrders(args.Mask);
        }


        [HttpDelete("[action]")]
        public async Task<List<Order>> DeleteOrder([FromBody] DeleteOrderArgs args)
        {
            OrderDto orderDto = _orderContext.Orders.Include(ord => ord.OrderLines).Single(ord => ord.Id == args.OrderId);
            _orderContext.RemoveRange(orderDto.OrderLines);
            _orderContext.Remove(orderDto);
            await _orderContext.SaveChangesAsync();
            return await GetOrders(args.Mask);
        }

        [HttpGet("Products")]
        public async Task<List<ProductDto>> GetProducts()
        {
            var result = await _orderContext.Products.ToListAsync();
            return result;
        }

        [HttpGet("OrderLines")]
        public async Task<List<OrderLine>> GetOrderLines(long orderId)
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
        public async Task<List<OrderLine>> AddOrderLine([FromBody] NewOrderLineArgs args)
        {       
            OrderLineDto orderLine = await _orderContext.OrderLines.SingleOrDefaultAsync(line => line.OrderId == args.OrderId
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

        [HttpDelete("[action]")]
        public async Task<List<OrderLine>> DeleteOrderLine([FromBody] DeleteOrderLineArgs args)
        {
            OrderLineDto orderLine  = await _orderContext.OrderLines.SingleOrDefaultAsync(oL => oL.Id == args.OrderLineId);
            if(orderLine == null)
            {
                throw new Exception($"Can not find order line with id {args.OrderLineId} and order with id {args.OrderId}");
            }

            _orderContext.OrderLines.Remove(orderLine);
            
            await _orderContext.SaveChangesAsync();
            return await GetOrderLines(args.OrderId);
        }

    }

    
}
