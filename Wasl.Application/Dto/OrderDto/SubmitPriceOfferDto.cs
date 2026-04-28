namespace Wasl.Application.Dto.OrderDto
{
    public class SubmitPriceOfferDto
    {
        // سعر الوحدة (جنيه مصري)
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero.")]
        public decimal UnitPrice { get; set; }

        // ملاحظات للتاجر (اختياري)
        public string? Notes { get; set; }
    }
}
