namespace OnlineShop.Data
{
    public class OrderLine
    {
        public long Id { get; set;}
        public long OrderId { get; set;}
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        
    }
}