namespace OrderProcessorService
{
    public class Order
    {
        public string OrderId { get; set; }
        public string CustomerName { get; set; }
        public string OrderDate { get; set; }
        public List<string> Items { get; set; }
        public double TotalAmount { get; set; }
        public bool IsHighValue { get; set; }
    }
}
