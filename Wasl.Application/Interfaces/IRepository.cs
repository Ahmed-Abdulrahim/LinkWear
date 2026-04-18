namespace Wasl.Application.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<T> entites, CancellationToken cancellationToken = default);
        void Update(T entity);
        void Delete(T entity);

        //Get entity
        Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<T> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
        Task<T> GetByIdSpecTrackedAsync(ISpecefication<T> spec, CancellationToken cancellationToken = default);
        Task<T> GetByIdSpecAsync(ISpecefication<T> spec, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllSpecTrackedAsync(ISpecefication<T> spec, CancellationToken cancellationToken = default);
    }
}
