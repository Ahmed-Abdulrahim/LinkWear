namespace Wasl.Application.Dto.ProductDto
{
    public class AddProductDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int AvailableQuantity { get; set; }
        public int? MinimumOrder { get; set; }
        public ActivityType? ActivityType { get; set; }

        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
