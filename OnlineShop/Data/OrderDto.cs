using System.Collections.Generic;

namespace OnlineShop.Data
{
    public class OrderDto
    {
        public long Id { get; set;}
        public string CustomerName {get; set;}
        public IList<OrderLineDto> OrderLines {get; set;}
    }
}