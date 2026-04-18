namespace Wasl.Infrastructure.Specifications
{
    public class BaseSpecification<T> : ISpecefication<T> where T : BaseEntity
    {
        public BaseSpecification()
        {

        }
        public BaseSpecification(Expression<Func<T, bool>> expression)
        {
            Criteria = expression;
        }
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDesc { get; set; }
        public List<string> IncludeString { get; set; } = new List<string>();
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool HasPagination { get; set; }

        //Order By 
        protected void AddOrderBy(Expression<Func<T, object>> expression)
             => OrderBy = expression;

        //OrderBy Desc
        protected void AddOrderByDesc(Expression<Func<T, object>> expression)
            => OrderByDesc = expression;

        //Iclude String For Loading Navigation Prop
        protected void AddInclude(string includeString)
            => IncludeString.Add(includeString);

        //Pagination Response
        protected void AddPagination(int skip, int take)
        {
            HasPagination = true;
            Skip = skip;
            Take = take;
        }
    }
}
