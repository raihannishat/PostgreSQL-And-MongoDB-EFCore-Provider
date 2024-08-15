namespace EFCoreForPostgreSQLAndMongoDB;

public static class ServiceExtensions
{
    public static void AddConfigurations(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDbContextFactory, DbContextFactory>();
        services.AddScoped<IRepositoryFactory, RepositoryFactory>();
    }
}
