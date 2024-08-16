namespace EFCoreForPostgreSQLAndMongoDB.Data;

public interface IDbContext : IDisposable
{
    DbSet<T> GetDbSet<T>() where T : class, IEntity;
    Task<int> CommitChangesAsync();
}
