namespace Wasl.Application.Validation
{
    public class RefreshTokenValidator : AbstractValidator<RefreshTokenDto>
    {
        public RefreshTokenValidator()
        {
            RuleFor(r => r.Token).NotEmpty().WithMessage("Token must not be empty");
            RuleFor(r => r.RefreshToken).NotEmpty().WithMessage("RefreshToken must not be empty");
        }
    }
}
