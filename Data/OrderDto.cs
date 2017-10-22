using System.Collections.Generic;

namespace OnlineShop.Data
{
    public class OrderDto
    {
        public int Id { get; set;}
        public string CustomerName {get; set;}
        public IList<OrderLineDto> OrderLines {get; set;}
    }
}