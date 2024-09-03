namespace EFCoreForPostgreSQLAndMongoDB.Data;

public interface IDbContext : IDisposable
{
    DbSet<T> GetDbSet<T>() where T : class, IEntity;

    Task<int> CommitChangesAsync(bool acceptAllChangesOnSuccess = true);

    Task<IDbContextTransaction> BeginTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();

    Task<T> FindByIdAsync<T>(params object[] keyValues) where T : class, IEntity;

    void Attach<T>(T entity) where T : class, IEntity;

    void Detach<T>(T entity) where T : class, IEntity;
}