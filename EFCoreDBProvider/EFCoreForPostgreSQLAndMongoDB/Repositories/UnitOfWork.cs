namespace EFCoreForPostgreSQLAndMongoDB.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
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
        _currentTransaction ??= await _context.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await EnsureTransactionAsync(async () =>
        {
            await _context.CommitChangesAsync();
            await _currentTransaction!.CommitAsync();
        });
    }

    public async Task RollbackTransactionAsync()
    {
        await EnsureTransactionAsync(async () =>
        {
            await _currentTransaction!.RollbackAsync();
        });
    }

    private async Task EnsureTransactionAsync(Func<Task> transactionAction)
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No transaction is active.");
        }

        try
        {
            await transactionAction();
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
        return (IRepository<T>)_repositories.GetOrAdd(typeof(T), _ =>
            _repositoryFactory.CreateRepository<T>(_context));
    }

    public async Task<int> CommitChangesAsync()
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("CommitChangesAsync should not be called directly when a transaction is active.");
        }

        return await _context.CommitChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            _repositories.Clear();
            _context.Dispose();

            try
            {
                Task.Run(DisposeTransactionAsync).GetAwaiter().GetResult();
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
