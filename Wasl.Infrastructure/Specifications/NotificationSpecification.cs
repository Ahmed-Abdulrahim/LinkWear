namespace Wasl.Infrastructure.Specifications
{
    public class NotificationSpecification : BaseSpecification<Notifications>
    {
        public NotificationSpecification(Guid userId, int page, int pageSize)
            : base(n => n.UserId == userId && !n.IsDeleted)
        {
            AddOrderByDesc(n => n.CreatedAt);
            AddPagination((page - 1) * pageSize, pageSize);
            AddInludes();
        }

        public NotificationSpecification(Expression<Func<Notifications, bool>> expression)
            : base(expression)
        {
            AddOrderByDesc(n => n.CreatedAt);
            AddInludes();
        }

        void AddInludes()
        {
            Includes.Add(n => n.User);
            Includes.Add(n => n.Order);

        }
    }
}
