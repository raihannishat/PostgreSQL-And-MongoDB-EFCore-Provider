namespace EFCoreForPostgreSQLAndMongoDB.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly IDbContext _context;
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(IDbContextFactory dbContextFactory, IRepositoryFactory repositoryFactory)
    {
        _context = dbContextFactory.GetDbContext();
        _repositoryFactory = repositoryFactory;
    }

    public IRepository<T> GetRepository<T>() where T : class, IEntity
    {
        var type = typeof(T);

        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = _repositoryFactory.CreateRepository<T>(_context);
        }

        return (IRepository<T>)_repositories[type];
    }

    public async Task<int> CommitChangesAsync()
    {
        return await _context.CommitChangesAsync();
    }

    public void Dispose()
    {
        _repositories.Clear();
        _context.DisposeConnection();
        GC.SuppressFinalize(this);
    }
}
