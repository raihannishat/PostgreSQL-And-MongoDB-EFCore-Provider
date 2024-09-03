
namespace EFCoreForPostgreSQLAndMongoDB.Data;

public class PostgreSqlDbContext : DbContext, IDbContext
{
    public PostgreSqlDbContext(DbContextOptions<PostgreSqlDbContext> options)
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

    public DbSet<T> GetDbSet<T>() where T : class, IEntity
    {
        return base.Set<T>();
    }

    public new void Dispose()
    {
        base.Dispose();
    }

    public async Task<int> CommitChangesAsync(bool acceptAllChangesOnSuccess = true)
    {
        return await SaveChangesAsync(acceptAllChangesOnSuccess);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await Database.BeginTransactionAsync();
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
        var entity = await Set<T>().FindAsync(keyValues);

        return entity == null
            ? throw new InvalidOperationException($"Entity of type {typeof(T).Name} with key values {string.Join(", ", keyValues)} not found.")
            : entity;
    }

    public new void Attach<T>(T entity) where T : class, IEntity
    {
        Entry(entity).State = EntityState.Unchanged;
    }

    public void Detach<T>(T entity) where T : class, IEntity
    {
        Entry(entity).State = EntityState.Detached;
    }
}
