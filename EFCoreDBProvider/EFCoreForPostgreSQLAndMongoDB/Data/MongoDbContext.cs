﻿namespace EFCoreForPostgreSQLAndMongoDB.Data;

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

    public Task<int> CommitChangesAsync()
    {
        return base.SaveChangesAsync();
    }

    public override void Dispose() => base.Dispose();
}
