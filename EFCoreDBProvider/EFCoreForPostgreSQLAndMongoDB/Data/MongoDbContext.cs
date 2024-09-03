namespace EFCoreForPostgreSQLAndMongoDB.Data;

public class MongoDbContext : DbContext, IDbContext
{
    public MongoDbContext(DbContextOptions<MongoDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var entityTypes = typeof(IEntity).Assembly.GetTypes()
            .Where(t => typeof(IEntity).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var type in entityTypes)
        {
            modelBuilder.Entity(type);
        }
    }

    public DbSet<T> GetDbSet<T>() where T : class, IEntity
    {
        return base.Set<T>();
    }

    public async Task<int> CommitChangesAsync(bool acceptAllChangesOnSuccess = true)
    {
        return await base.SaveChangesAsync();
    }

    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        // MongoDB does not support transactions in the same way as SQL databases.
        // This method can be left as a no-op or throw a NotSupportedException.
        throw new NotSupportedException("MongoDB does not support transactions in the same way as SQL databases.");
    }

    public Task CommitTransactionAsync()
    {
        // MongoDB does not support transactions in the same way as SQL databases.
        // This method can be left as a no-op or throw a NotSupportedException.
        throw new NotSupportedException("MongoDB does not support transactions in the same way as SQL databases.");
    }

    public Task RollbackTransactionAsync()
    {
        // MongoDB does not support transactions in the same way as SQL databases.
        // This method can be left as a no-op or throw a NotSupportedException.
        throw new NotSupportedException("MongoDB does not support transactions in the same way as SQL databases.");
    }

    public async Task<T?> FindByIdAsync<T>(params object[] keyValues) where T : class, IEntity
    {
        var dbSet = GetDbSet<T>() ?? throw new InvalidOperationException($"DbSet for type '{typeof(T).Name}' is not configured.");

        // Find the entity by its key values and return the result
        return await dbSet.FindAsync(keyValues).AsTask().ConfigureAwait(false);
    }

    public new void Attach<T>(T entity) where T : class, IEntity
    {
        Entry(entity).State = EntityState.Unchanged;
    }

    public void Detach<T>(T entity) where T : class, IEntity
    {
        Entry(entity).State = EntityState.Detached;
    }

    public new void Dispose()
    {
        base.Dispose();
    }
}
