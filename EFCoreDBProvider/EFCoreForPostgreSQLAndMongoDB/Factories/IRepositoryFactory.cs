namespace EFCoreForPostgreSQLAndMongoDB.Factories;

public interface IRepositoryFactory
{
    IRepository<T> CreateRepository<T>(IDbContext context) where T : class, IEntity;
}
