namespace Wasl.Application.Dto.ProductDto
{
    public class UpdateProductDto
    {
        public Guid ProductId { get; set; }
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
        public int? MinimumOrder { get; set; }
    }
}
