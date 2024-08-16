namespace EFCoreForPostgreSQLAndMongoDB.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private bool _isDisposed;
    private readonly IDbContext _context;
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public UnitOfWork(IDbContextFactory dbContextFactory, IRepositoryFactory repositoryFactory)
    {
        _context = dbContextFactory.GetDbContext();
        _repositoryFactory = repositoryFactory;
    }

    public IRepository<T> GetRepository<T>() where T : class, IEntity
    {
        var type = typeof(T);

        return (IRepository<T>)_repositories
            .GetOrAdd(type, _ => _repositoryFactory.CreateRepository<T>(_context));
    }

    public async Task<int> CommitChangesAsync()
    {
        return await _context.CommitChangesAsync();
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _repositories.Clear();
            _context.Dispose();
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
