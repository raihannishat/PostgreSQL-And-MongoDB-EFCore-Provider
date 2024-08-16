namespace EFCoreForPostgreSQLAndMongoDB;

public static class ServiceExtensions
{
    public static void AddConfigurations(this IServiceCollection services, IHostApplicationBuilder builder)
    {
        // Register settings for mongodb context
        var mongoDbSettings = builder.Configuration.GetSection(typeof(MongoDbSettings).Name).Get<MongoDbSettings>();
        builder.Services.AddDbContext<MongoDbContext>(options =>
            options.UseMongoDB(mongoDbSettings!.AtlasURI, mongoDbSettings.DatabaseName));

        // Register settings for pgsqldb context
        var pgsqlSettings = builder.Configuration.GetSection(typeof(PgsqlDbSettings).Name).Get<PgsqlDbSettings>();
        builder.Services.AddDbContext<PostgreSqlDbContext>(options =>
            options.UseNpgsql(pgsqlSettings!.ConnectionString));

        // Register srevice, factory and UoW
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDbContextFactory, DbContextFactory>();
        services.AddScoped<IRepositoryFactory, RepositoryFactory>();
    }
}
