namespace Wasl.Domain.Entites
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
        public int? MinimumOrder { get; set; }
        public ActivityType? ActivityType { get; set; }
        public Guid SupplierId { get; set; }

        //Nvaigation
        public ApplicationUser Supplier { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
