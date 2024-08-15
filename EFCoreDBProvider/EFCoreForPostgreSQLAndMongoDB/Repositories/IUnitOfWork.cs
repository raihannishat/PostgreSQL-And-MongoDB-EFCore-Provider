﻿namespace EFCoreForPostgreSQLAndMongoDB.Repositories;

public interface IUnitOfWork
{
    IRepository<T> GetRepository<T>() where T : class, IEntity;
    Task<int> CommitChangesAsync();
}
