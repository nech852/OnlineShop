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
using System.Threading;


namespace Test
{
    [TestClass]
    public class OrderControllerTests
    {
        private OrderController _orderController;
        private OrderContext _orderContext;

        [ClassInitialize]
        public static async  Task ClassInitialize(TestContext testContext)
        {
            OrderContext orderContext = CreateOrderContext();
            await orderContext.Database.EnsureDeletedAsync();
            await orderContext.Database.MigrateAsync();
            await  DbInitializer.InitializeAsync(orderContext);
            orderContext.Dispose();
        }
        
        [ClassCleanup]
        public static async  Task ClassCleanup()
        {
            OrderContext orderContext = CreateOrderContext();
            await orderContext.Database.EnsureDeletedAsync();
            orderContext.Dispose();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _orderContext = CreateOrderContext();
            _orderController = new OrderController(_orderContext);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _orderContext.Dispose();
        }

        [TestMethod]
        public async Task DeleteOrderTest()
        {
           string customerName = "DeleteOrderCustomer";
            List<ProductDto> products = await _orderController.GetProducts();
            ProductDto product1 = products[0];
            ProductDto product2 = products[2];

            var newOrderArgs = new NewOrderArgs
            {
                CustomerName = customerName
            };
            List<Order> orders = await _orderController.AddOrder(newOrderArgs);

            Order order = orders.FirstOrDefault(ord => ord.CustomerName == customerName);

            var newOrderLineArgs1 = new NewOrderLineArgs
            {
                OrderId = order.Id,
                ProductId = product1.Id,
                Quantity = 8
            };
            var newOrderLineArgs2 = new NewOrderLineArgs
            {
                OrderId = order.Id,
                ProductId = product2.Id,
                Quantity = 9
            };
            
            await _orderController.AddOrderLine(newOrderLineArgs1);
            await _orderController.AddOrderLine(newOrderLineArgs2);
            var deleteOrderArgs = new DeleteOrderArgs
            {
                OrderId = order.Id  
            };
            orders = await _orderController.DeleteOrder(deleteOrderArgs);
            Assert.IsNull(orders.FirstOrDefault(ord => ord.CustomerName == customerName));
        }

        [TestMethod]
        public async Task AddOrderTest()
        {
            string customerName = "AddOrderCustomer";
            List<ProductDto> products = await _orderController.GetProducts();
            ProductDto product = products[0];
            List<Order> orders = await _orderController.GetOrders();
            Assert.IsNull(orders.FirstOrDefault(ord => ord.CustomerName == customerName));
            var newOrderArgs = new NewOrderArgs
            {
                CustomerName = customerName
            };
            orders = await _orderController.AddOrder(newOrderArgs);
            Order order = orders.FirstOrDefault(ord => ord.CustomerName == customerName);
            Assert.IsNotNull(order);
            List<Order> ordersFromGetMethod = await _orderController.GetOrders();
            Assert.AreEqual(orders.Count, ordersFromGetMethod.Count);
            Order orderFromGetMethod = ordersFromGetMethod.FirstOrDefault(ord => ord.CustomerName == customerName);
            Assert.AreEqual(order.Id, orderFromGetMethod.Id);
        }

        [TestMethod]
        public async Task AddOrderLineTest()
        {
            string customerName = "AddOrderLineTestCustomer";
            List<ProductDto> products = await _orderController.GetProducts();
            ProductDto product1 = products[0];
            ProductDto product2 = products[2];

            var newOrderArgs = new NewOrderArgs
            {
                CustomerName = customerName
            };
            List<Order> orders = await _orderController.AddOrder(newOrderArgs);

            Order order = orders.FirstOrDefault(ord => ord.CustomerName == customerName);

            var newOrderLineArgs1 = new NewOrderLineArgs
            {
                OrderId = order.Id,
                ProductId = product1.Id,
                Quantity = 8
            };
            var newOrderLineArgs2 = new NewOrderLineArgs
            {
                OrderId = order.Id,
                ProductId = product2.Id,
                Quantity = 9
            };
            var newOrderLineArgs3 = new NewOrderLineArgs
            {
                OrderId = order.Id,
                ProductId = product1.Id,
                Quantity = 7
            };
            await _orderController.AddOrderLine(newOrderLineArgs1);
            await _orderController.AddOrderLine(newOrderLineArgs2);
            List<OrderLine> orderLines = await _orderController.AddOrderLine(newOrderLineArgs3);
            Assert.AreEqual(2, orderLines.Count);

            OrderLine orderLine1 = orderLines.First(ordLine => ordLine.ProductId == product1.Id);
            Assert.AreEqual(order.Id, orderLine1.OrderId);
            Assert.AreEqual(product1.Id, orderLine1.ProductId);
            Assert.AreEqual(15,  orderLine1.Quantity);
            Assert.IsFalse(string.IsNullOrEmpty(orderLine1.ProductName));
            Assert.IsTrue(orderLine1.ProductPrice > 0);
            Assert.IsTrue(orderLine1.Id > 0);

            OrderLine orderLine2 = orderLines.First(ordLine => ordLine.ProductId == product2.Id);
            Assert.AreEqual(order.Id, orderLine2.OrderId);
            Assert.AreEqual(product2.Id, orderLine2.ProductId);
            Assert.AreEqual(9,  orderLine2.Quantity);
            Assert.IsFalse(string.IsNullOrEmpty(orderLine2.ProductName));
            Assert.IsTrue(orderLine2.ProductPrice > 0);
            Assert.IsTrue(orderLine2.Id > 0);

            orders = await _orderController.GetOrders();
            order = orders.FirstOrDefault(ord => ord.CustomerName == customerName);
            decimal totalPrice = product1.Price * 15 + product2.Price * 9;
            Assert.AreEqual(totalPrice, order.TotalPrice);
        }
        
        [TestMethod] 
        public async Task DeleteOrderLine()
        {
           string customerName = "DeleteOrderLineCustomer";
            List<ProductDto> products = await _orderController.GetProducts();
            ProductDto product1 = products[0];
            ProductDto product2 = products[2];

            var newOrderArgs = new NewOrderArgs
            {
                CustomerName = customerName
            };
            List<Order> orders = await _orderController.AddOrder(newOrderArgs);

            Order order = orders.FirstOrDefault(ord => ord.CustomerName == customerName);

            var newOrderLineArgs1 = new NewOrderLineArgs
            {
                OrderId = order.Id,
                ProductId = product1.Id,
                Quantity = 8
            };
            var newOrderLineArgs2 = new NewOrderLineArgs
            {
                OrderId = order.Id,
                ProductId = product2.Id,
                Quantity = 9
            };
            
            await _orderController.AddOrderLine(newOrderLineArgs1);
            List<OrderLine> orderLines = await _orderController.AddOrderLine(newOrderLineArgs2);
            
            orders = await _orderController.AddOrder(newOrderArgs);
            order = orders.FirstOrDefault(ord => ord.CustomerName == customerName);
            OrderLine orderLine = orderLines.First(oL => oL.OrderId == order.Id && oL.ProductId == product1.Id);

            var deleteArgs = new DeleteOrderLineArgs
            {
                OrderId = order.Id,
                OrderLineId = orderLine.Id
            };
            List<OrderLine> orderLinesAfterDeleting = await _orderController.DeleteOrderLine(deleteArgs);

            Assert.AreEqual(1, orderLinesAfterDeleting.Count);

            OrderLine orderLineAfterDeleting = orderLinesAfterDeleting[0];
            Assert.AreEqual(order.Id, orderLineAfterDeleting.OrderId);
            Assert.AreEqual(product2.Id, orderLineAfterDeleting.ProductId);
            Assert.AreEqual(9,  orderLineAfterDeleting.Quantity);
            Assert.IsFalse(string.IsNullOrEmpty(orderLineAfterDeleting.ProductName));
            Assert.IsTrue(orderLineAfterDeleting.ProductPrice > 0);
            Assert.IsTrue(orderLineAfterDeleting.Id > 0);

            List<OrderLine> orderLinesFromGetMethod = await _orderController.GetOrderLines(order.Id);

            Assert.AreEqual(1, orderLinesFromGetMethod.Count);

            OrderLine orderLineFromGetMethod = orderLinesFromGetMethod[0];
            Assert.AreEqual(order.Id, orderLineFromGetMethod.OrderId);
            Assert.AreEqual(product2.Id, orderLineFromGetMethod.ProductId);
            Assert.AreEqual(9, orderLineFromGetMethod.Quantity);
            Assert.AreEqual(orderLineAfterDeleting.ProductName, orderLineFromGetMethod.ProductName);
            Assert.AreEqual(orderLineAfterDeleting.ProductPrice, orderLineFromGetMethod.ProductPrice);
            Assert.AreEqual(orderLineAfterDeleting.Id, orderLineFromGetMethod.Id);

            List<Order> ordersFromGetMethod = await _orderController.GetOrders();
            Assert.AreEqual(orders.Count, ordersFromGetMethod.Count);
            Order orderFromGetMethod = ordersFromGetMethod.FirstOrDefault(ord => ord.CustomerName == customerName);

            decimal totalPrice = product2.Price * 9;
            Assert.AreEqual(totalPrice, orderFromGetMethod.TotalPrice);
        }

        //TODO: Search by mask - check  total sum after deleteing??

        private static OrderContext CreateOrderContext()
        {
          IConfiguration configuration = GetConfig();

            string connectionString = configuration.GetConnectionString("DefaultConnection");
            
            var optionsBuilder = new DbContextOptionsBuilder<OrderContext>();
            optionsBuilder.UseSqlServer(connectionString);
            
            return new OrderContext(optionsBuilder.Options);
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
