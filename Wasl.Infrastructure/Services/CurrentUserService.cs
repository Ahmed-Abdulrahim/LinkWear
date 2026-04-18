namespace Wasl.Infrastructure.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {

        public string? UserId =>
            httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string? Email =>
            httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.Email)?.Value;

        public string? Role =>
            httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.Role)?.Value;

        public bool IsAuthenticated =>
            httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        public bool IsDeleted =>
            httpContextAccessor.HttpContext?.User?
                .FindFirst("IsDeleted")?.Value == "true";
    }
}
