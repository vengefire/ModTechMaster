using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using DapperExtensions;
using Framework.Interfaces.Repositories;

namespace Framework.Logic.Repository
{
    public class DapperRepositoryBase<TEntity> : IDapperRepository<TEntity>
        where TEntity : class
    {
        public DapperRepositoryBase(IDbConnection dbConnection)
        {
            DbConnection = dbConnection;
            DbConnection.Open();
        }

        public IDbConnection DbConnection { get; }

        public dynamic Create(TEntity entity)
        {
            DbConnection.Insert(entity);
            dynamic identity = DbConnection.Query("SELECT @@IDENTITY AS Id").Single();
            return identity.Id;
        }

        public void Delete(TEntity entity)
        {
            DbConnection.Delete(entity);
        }

        public IEnumerable<TEntity> FetchAll(object predicate, IList<ISort> sort = null)
        {
            return DbConnection.GetList<TEntity>(predicate, sort);
        }

        public void Dispose()
        {
            DbConnection.Close();
            DbConnection.Dispose();
        }

        public IEnumerable<TEntity> FetchAll()
        {
            return DbConnection.GetList<TEntity>();
        }

        public TEntity FetchByKey(dynamic key)
        {
            return DapperExtensions.DapperExtensions.Get<TEntity>(DbConnection, key);
        }

        public void Update(TEntity entity)
        {
            DbConnection.Update(entity);
        }
    }
}