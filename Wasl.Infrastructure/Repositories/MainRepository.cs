namespace Wasl.Infrastructure.Repositories
{
    public class MainRepository<T>(WaslDbContext context) : IRepository<T> where T : BaseEntity
    {
        private DbSet<T> Set = context.Set<T>();

        // Main Func
        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
            => await Set.AddAsync(entity, cancellationToken);

        public async Task AddRangeAsync(IEnumerable<T> entites, CancellationToken cancellationToken = default)
            => await Set.AddRangeAsync(entites, cancellationToken);
        public void Delete(T entity)
            => Set.Remove(entity);
        public void Update(T entity)
            => Set.Update(entity);


        // Get Entity
        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
            => await Set.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<IEnumerable<T>> GetAllSpecTrackedAsync(ISpecefication<T> spec, CancellationToken cancellationToken = default)

           => await GenerateQuery(spec).ToListAsync(cancellationToken);


        public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await Set.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);


        public async Task<T> GetByIdSpecTrackedAsync(ISpecefication<T> spec, CancellationToken cancellationToken = default)

            => await GenerateQuery(spec).FirstOrDefaultAsync(cancellationToken);

        public async Task<T> GetByIdSpecAsync(ISpecefication<T> spec, CancellationToken cancellationToken = default)

            => await GenerateQuery(spec).AsNoTracking().FirstOrDefaultAsync(cancellationToken);

        public async Task<T> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default)
            => await Set.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        //GetQuery Func
        IQueryable<T> GenerateQuery(ISpecefication<T> spec) => SpecificationEvaluator<T>.GetQuery(Set, spec);

    }
}
