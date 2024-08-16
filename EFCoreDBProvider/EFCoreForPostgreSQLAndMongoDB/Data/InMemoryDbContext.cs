﻿namespace EFCoreForPostgreSQLAndMongoDB.Data;

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

    public Task<int> CommitChangesAsync()
    {
        return base.SaveChangesAsync();
    }

    public DbSet<T> GetDbSet<T>() where T : class, IEntity
    {
        return base.Set<T>();
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}
