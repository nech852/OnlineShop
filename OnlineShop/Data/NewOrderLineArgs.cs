namespace OnlineShop.Data
{       
    public class NewOrderLineArgs
    {
        public long OrderId { get; set;}
        public int ProductId { get; set;}
        public int Quantity {get; set; }
    }
}