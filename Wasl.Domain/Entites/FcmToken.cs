namespace Wasl.Domain.Entites
{
    public class FcmToken : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }

        //Navigation
        public ApplicationUser User { get; set; }

    }
}

