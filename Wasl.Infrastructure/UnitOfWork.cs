namespace Wasl.Infrastructure
{
    public class UnitOfWork(WaslDbContext context) : IUnitOfWork
    {
        private readonly Dictionary<string, object> repositories = new();

        public IRepository<T> Repository<T>() where T : BaseEntity
        {

            var key = typeof(T).Name;
            if (!repositories.ContainsKey(key))
            {
                var repo = new MainRepository<T>(context);
                repositories.Add(key, repo);
            }

            return (IRepository<T>)repositories[key];
        }
        public async Task<int> CommitAsync(CancellationToken cancellationToken) => await context.SaveChangesAsync(cancellationToken);

        public void Dispose() => context.Dispose();

    }
}
