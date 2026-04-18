namespace Wasl.Domain.Entites
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string? Description { get; set; }

    }
}
