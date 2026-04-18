namespace Wasl.Infrastructure.Specifications
{
    public class ProductSpecification : BaseSpecification<Product>
    {
        public ProductSpecification() : base()
        {
            AddIncludes();
        }
        public ProductSpecification(Guid id) : base(rt => rt.Id == id)
        {
            AddIncludes();
        }
        public ProductSpecification(Expression<Func<Product, bool>> expression) : base(expression)
        {
            AddIncludes();
        }
        void AddIncludes()
        {
            Includes.Add(rt => rt.OrderItems);
            Includes.Add(rt => rt.Supplier);

        }
    }
}
