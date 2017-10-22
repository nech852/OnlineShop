namespace OnlineShop.Data
{
    public class OrderLineDto
    {
        public int Id { get; set;}
        public int OrderId { get; set;}
        public int ProductId {get; set;}
        public int Quantity {get; set;}
        public OrderDto Order { get; set; }
        public Product Product {get; set;}
    }
}