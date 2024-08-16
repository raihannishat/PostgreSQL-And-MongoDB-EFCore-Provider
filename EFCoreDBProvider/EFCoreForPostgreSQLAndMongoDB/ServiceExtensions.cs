namespace EFCoreForPostgreSQLAndMongoDB;

public static class ServiceExtensions
{
    public static void AddConfigurations(this IServiceCollection services, IHostApplicationBuilder builder)
    {
        // Register settings for context settings
        services.Configure<ContextSettings>(builder.Configuration.GetSection(typeof(ContextSettings).Name));

        // Register In-Memory context
        services.AddDbContext<InMemoryDbContext>(options =>
        {
            var inMemorySettings = builder.Configuration.GetSection(typeof(InMemorySettings).Name).Get<InMemorySettings>();
            options.UseInMemoryDatabase(inMemorySettings!.DatabaseName);
        });

        // Register MongoDB context
        services.AddDbContext<MongoDbContext>(options =>
        {
            var mongoDbSettings = builder.Configuration.GetSection(typeof(MongoDbSettings).Name).Get<MongoDbSettings>();
            options.UseMongoDB(mongoDbSettings!.AtlasURI, mongoDbSettings.DatabaseName);
        });

        // Register PostgreSQL context
        services.AddDbContext<PostgreSqlDbContext>(options =>
        {
            var pgsqlSettings = builder.Configuration.GetSection(typeof(PgsqlDbSettings).Name).Get<PgsqlDbSettings>();
            options.UseNpgsql(pgsqlSettings!.ConnectionString);
        });

        // Register services, factories, and Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDbContextFactory, DbContextFactory>();
        services.AddScoped<IRepositoryFactory, RepositoryFactory>();
    }
}
