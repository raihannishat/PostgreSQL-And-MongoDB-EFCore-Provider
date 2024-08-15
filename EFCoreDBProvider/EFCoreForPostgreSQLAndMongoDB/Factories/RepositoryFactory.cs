namespace EFCoreForPostgreSQLAndMongoDB.Factories;

public class RepositoryFactory : IRepositoryFactory
{
    private readonly IServiceProvider _serviceProvider;

    public RepositoryFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IRepository<T> CreateRepository<T>(IDbContext context) where T : class, IEntity
    {
        return context == null
            ? throw new ArgumentNullException(nameof(context))
            : (IRepository<T>)ActivatorUtilities.CreateInstance<Repository<T>>(_serviceProvider, context);
    }
}