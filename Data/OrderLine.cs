namespace OnlineShop.Data
{
    public class OrderLine
    {
        public int Id { get; set;}
        public int OrderId { get; set;}
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public float ProductPrice { get; set; }
        public int Quantity { get; set; }
        
    }
}