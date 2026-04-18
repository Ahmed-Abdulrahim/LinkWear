namespace Wasl.Application.Validation
{
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        private static readonly string[] ValidRoles = { "StoreOwner", "Supplier" };

        public RegisterValidator()
        {


            RuleFor(r => r.Email).NotEmpty().WithMessage("Email must not be Empty")
                .EmailAddress().WithMessage("Invalid Email format")
                .MaximumLength(256).WithMessage("Email Cannot Exceed 100 Character ");


            RuleFor(r => r.Password).NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).MaximumLength(100).WithMessage("Password must be between 8 and 100 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number and one special character");

            RuleFor(r => r.ConfirmPassword).NotEmpty().WithMessage("Confirm Password is required")
                .Equal(r => r.Password).WithMessage("Password and confirmation password do not match");
            RuleFor(x => x.Role)
               .NotEmpty()
               .Must(r => ValidRoles.Contains(r))
               .WithMessage("Role must be one of: StoreOwner, Supplier.");
        }
    }
}
