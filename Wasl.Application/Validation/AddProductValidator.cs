namespace Wasl.Application.Validation
{
    public class AddProductValidator : AbstractValidator<AddProductDto>
    {
        public AddProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("اسم المنتج مطلوب")
                .MaximumLength(200).WithMessage("اسم المنتج لا يمكن أن يتجاوز 200 حرف");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("السعر يجب أن يكون أكبر من صفر");

            RuleFor(x => x.AvailableQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("الكمية المتاحة لا يمكن أن تكون سالبة");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("وصف المنتج لا يمكن أن يتجاوز 1000 حرف");
        }
    }
}
