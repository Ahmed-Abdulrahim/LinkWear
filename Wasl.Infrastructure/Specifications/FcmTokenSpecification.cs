namespace Wasl.Infrastructure.Specifications
{
    public class FcmTokenSpecification : BaseSpecification<FcmToken>
    {
        public FcmTokenSpecification(Guid userId)
            : base(t => t.UserId == userId && !t.IsDeleted)
        {
            AddInludes();
        }

        public FcmTokenSpecification(Guid userId, string token)
            : base(t => t.UserId == userId && t.Token == token && !t.IsDeleted)
        {
            AddInludes();
        }

        public FcmTokenSpecification(Expression<Func<FcmToken, bool>> expression)
            : base(expression)
        {
            AddInludes();
        }
        void AddInludes()
        {
            Includes.Add(f => f.User);
        }
    }
}
