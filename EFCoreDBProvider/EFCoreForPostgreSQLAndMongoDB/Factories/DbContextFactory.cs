namespace EFCoreForPostgreSQLAndMongoDB.Factories;

public class DbContextFactory : IDbContextFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ContextSettings _contextSettings;

    public DbContextFactory(IServiceProvider serviceProvider, IOptions<ContextSettings> contextSettings)
    {
        _serviceProvider = serviceProvider;
        _contextSettings = contextSettings.Value;
    }

    public IDbContext GetDbContext()
    {
        var useDbContext = _contextSettings.UseDbContext?.ToLowerInvariant();

        return useDbContext switch
        {
            "mongo" => _serviceProvider.GetRequiredService<MongoDbContext>(),
            "pgsql" => _serviceProvider.GetRequiredService<PostgreSqlDbContext>(),
            _ => _serviceProvider.GetRequiredService<InMemoryDbContext>()
        };
    }
}