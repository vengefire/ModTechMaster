namespace Framework.Logic.Repository
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DapperExtensions;
    using Interfaces.Repositories;

    public class DapperRepositoryBase<TEntity> : IDapperRepository<TEntity>
        where TEntity : class
    {
        public DapperRepositoryBase(IDbConnection dbConnection)
        {
            this.DbConnection = dbConnection;
            this.DbConnection.Open();
        }

        public IDbConnection DbConnection { get; }

        public dynamic Create(TEntity entity)
        {
            this.DbConnection.Insert(entity);
            var identity = this.DbConnection.Query("SELECT @@IDENTITY AS Id").Single();
            return identity.Id;
        }

        public void Delete(TEntity entity)
        {
            this.DbConnection.Delete(entity);
        }

        public IEnumerable<TEntity> FetchAll(object predicate, IList<ISort> sort = null)
        {
            return this.DbConnection.GetList<TEntity>(predicate, sort);
        }

        public void Dispose()
        {
            this.DbConnection.Close();
            this.DbConnection.Dispose();
        }

        public IEnumerable<TEntity> FetchAll()
        {
            return this.DbConnection.GetList<TEntity>();
        }

        public TEntity FetchByKey(dynamic key)
        {
            return DapperExtensions.Get<TEntity>(this.DbConnection, key);
        }

        public void Update(TEntity entity)
        {
            this.DbConnection.Update(entity);
        }
    }
}