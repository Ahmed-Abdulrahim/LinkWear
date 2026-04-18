namespace Wasl.Application.Validation
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.SupplierId)
                .NotEmpty().WithMessage("يجب تحديد المورد");

            RuleFor(x => x.Items)
                .NotNull().WithMessage("يجب إضافة منتج واحد على الأقل")
                .Must(items => items != null && items.Count > 0)
                .WithMessage("يجب إضافة منتج واحد على الأقل");

            RuleFor(x => x.DeliveryAddress)
                .MaximumLength(500).WithMessage("عنوان التسليم لا يمكن أن يتجاوز 500 حرف");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId)
                    .NotEmpty().WithMessage("يجب تحديد المنتج");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("الكمية يجب أن تكون أكبر من صفر");
            });
        }
    }
}
