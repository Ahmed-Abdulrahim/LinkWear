namespace Wasl.Infrastructure.Specifications
{
    public class OrderSpecification : BaseSpecification<Order>
    {
        public OrderSpecification() : base()
        {
            AddIncludes();
        }
        public OrderSpecification(Guid id) : base(rt => rt.Id == id)
        {
            AddIncludes();
        }
        public OrderSpecification(Expression<Func<Order, bool>> expression) : base(expression)
        {
            AddIncludes();
        }
        void AddIncludes()
        {
            Includes.Add(o => o.StoreOwner);
            Includes.Add(o => o.Supplier);
            Includes.Add(o => o.Items);
            Includes.Add(o => o.TrackingHistory);

            IncludeString.Add("Items.Product");
        }
    }
}
