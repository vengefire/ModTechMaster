using System;
using System.Collections.Generic;
using System.Data;

namespace Framework.Interfaces.Repositories
{
    public interface IRepository<TEntity> : IDisposable
        where TEntity : class
    {
        IDbConnection DbConnection { get; }

        IEnumerable<TEntity> FetchAll();

        TEntity FetchByKey(dynamic key);

        void Delete(TEntity entity);

        dynamic Create(TEntity entity);

        void Update(TEntity entity);
    }
}