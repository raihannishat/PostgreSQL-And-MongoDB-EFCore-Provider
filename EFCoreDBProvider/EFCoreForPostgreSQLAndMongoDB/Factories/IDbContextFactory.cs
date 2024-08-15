namespace EFCoreForPostgreSQLAndMongoDB.Factories;

public interface IDbContextFactory
{
    IDbContext GetDbContext();
}