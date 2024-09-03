namespace EFCoreForPostgreSQLAndMongoDB.Data;

public class InMemoryDbContext : DbContext, IDbContext
{
    public InMemoryDbContext(DbContextOptions<InMemoryDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var entityTypes = typeof(IEntity).Assembly.GetTypes()
            .Where(t => typeof(IEntity).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var type in entityTypes)
        {
            modelBuilder.Entity(type);
        }
    }

    public async Task<int> CommitChangesAsync(bool acceptAllChangesOnSuccess = true)
    {
        return await SaveChangesAsync(acceptAllChangesOnSuccess);
    }

    public DbSet<T> GetDbSet<T>() where T : class, IEntity
    {
        return base.Set<T>();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        // In-memory databases do not support transactions in the same way as relational databases.
        // For demonstration purposes, we simulate transaction behavior.
        return await Task.FromResult(Database.BeginTransaction()); // No actual transaction support in InMemory.
    }

    public async Task CommitTransactionAsync()
    {
        if (Database.CurrentTransaction != null)
        {
            await Database.CommitTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (Database.CurrentTransaction != null)
        {
            await Database.RollbackTransactionAsync();
        }
    }

    public async Task<T> FindByIdAsync<T>(params object[] keyValues) where T : class, IEntity
    {
        var dbSet = Set<T>() ?? throw new InvalidOperationException($"DbSet for type '{typeof(T).Name}' is not configured.");

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
