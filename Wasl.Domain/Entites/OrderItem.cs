namespace Wasl.Domain.Entites
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Details { get; set; }

        //Navigation
        public Order Order { get; set; }
        public Product Product { get; set; }

    }
}
