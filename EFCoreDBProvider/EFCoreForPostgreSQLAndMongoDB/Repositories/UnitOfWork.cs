namespace EFCoreForPostgreSQLAndMongoDB.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbContext _context;
    private readonly IRepositoryFactory _repositoryFactory;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _currentTransaction;
    private bool _isDisposed;

    public UnitOfWork(IDbContextFactory dbContextFactory, IRepositoryFactory repositoryFactory)
    {
        _context = dbContextFactory.GetDbContext();
        _repositoryFactory = repositoryFactory;
    }

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction == null)
        {
            _currentTransaction = await _context.BeginTransactionAsync();
        }
    }

    public async Task CommitTransactionAsync()
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No transaction is active.");
        }

        try
        {
            await _context.CommitChangesAsync();
            await _currentTransaction.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            try
            {
                await _currentTransaction.RollbackAsync();
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }
    }

    private async Task DisposeTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public IRepository<T> GetRepository<T>() where T : class, IEntity
    {
        var type = typeof(T);
        return (IRepository<T>)_repositories.GetOrAdd(type, _ => 
            _repositoryFactory.CreateRepository<T>(_context));
    }

    public async Task<int> CommitChangesAsync()
    {
        return await _context.CommitChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _repositories.Clear();
                _context.Dispose();

                // Dispose of any remaining transactions
                try
                {
                    // Use Task.Run to call async method in sync context
                    Task.Run(async () => await DisposeTransactionAsync()).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it accordingly
                    Console.WriteLine($"Exception during Dispose: {ex.Message}");
                }
            }

            _isDisposed = true;
        }
    }
}