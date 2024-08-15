namespace EFCoreForPostgreSQLAndMongoDB.Factories;

public class DbContextFactory : IDbContextFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public DbContextFactory(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public IDbContext GetDbContext()
    {
        return _configuration.GetValue<bool>("UseMongoDb") switch
        {
            true => _serviceProvider.GetRequiredService<MongoDbContext>(),
            false => _serviceProvider.GetRequiredService<PostgreSqlDbContext>(),
        };
    }
}