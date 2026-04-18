namespace Wasl.Infrastructure.Specifications
{
    public static class SpecificationEvaluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecefication<T> spec)
        {
            var query = inputQuery;

            //Add Criteria is Found
            if (spec.Criteria is not null)
            {
                query = query.Where(spec.Criteria);
            }

            //Add OrderBy is Found
            if (spec.OrderBy is not null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDesc is not null)
            {
                query = query.OrderByDescending(spec.OrderByDesc);
            }


            // Now Add Includes To Current Query 
            query = spec.Includes
                .Aggregate(query, (currentQuery, IncludeExpression)
                => currentQuery.Include(IncludeExpression));

            // Add string-based includes (for nested)
            query = spec.IncludeString
                .Aggregate(query, (currentQuery, includeString)
                => currentQuery.Include(includeString));

            //Pagination
            if (spec.HasPagination)
                query = query.Skip(spec.Skip).Take(spec.Take);

            return query;


        }
    }
}
