namespace Wasl.Infrastructure.Specifications
{
    public class RefreshTokenSpecification : BaseSpecification<RefreshToken>
    {
        public RefreshTokenSpecification() : base()
        {
            AddIncludes();
        }
        public RefreshTokenSpecification(Guid id) : base(rt => rt.Id == id)
        {
            AddIncludes();
        }
        public RefreshTokenSpecification(Expression<Func<RefreshToken, bool>> expression) : base(expression)
        {
            AddIncludes();
        }
        void AddIncludes()
        {
            Includes.Add(rt => rt.ApplicationUser);

        }
    }
}
