namespace EFCoreForPostgreSQLAndMongoDB;

public static class ServiceExtensions
{
    public static void AddConfigurations(this IServiceCollection services, IHostApplicationBuilder builder)
    {
        // Register settings for context settings
        services.Configure<ContextSettings>(builder.Configuration.GetSection(typeof(ContextSettings).Name));

        // Register settings for in-memory context
        var inMemorySettings = builder.Configuration
            .GetSection(typeof(InMemorySettings).Name).Get<InMemorySettings>();

        services.AddDbContext<InMemoryDbContext>(options =>
            options.UseInMemoryDatabase(inMemorySettings!.DatabaseName));

        // Register settings for mongodb context
        var mongoDbSettings = builder.Configuration
            .GetSection(typeof(MongoDbSettings).Name).Get<MongoDbSettings>();
        
        services.AddDbContext<MongoDbContext>(options => 
            options.UseMongoDB(mongoDbSettings!.AtlasURI, mongoDbSettings.DatabaseName));

        // Register settings for pgsqldb context
        var pgsqlSettings = builder.Configuration
            .GetSection(typeof(PgsqlDbSettings).Name).Get<PgsqlDbSettings>();
        
        services.AddDbContext<PostgreSqlDbContext>(options =>
            options.UseNpgsql(pgsqlSettings!.ConnectionString));

        // Register srevice, factory and UoW
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDbContextFactory, DbContextFactory>();
        services.AddScoped<IRepositoryFactory, RepositoryFactory>();
    }
}
