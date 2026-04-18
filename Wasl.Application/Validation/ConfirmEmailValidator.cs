namespace Wasl.Application.Validation
{
    public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailDto>
    {
        public ConfirmEmailValidator()
        {
            RuleFor(c => c.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(c => c.Token)
                .NotEmpty().WithMessage("Token is required.");
        }
    }
}
