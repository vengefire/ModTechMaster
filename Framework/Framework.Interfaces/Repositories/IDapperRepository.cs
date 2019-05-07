using System.Collections.Generic;
using DapperExtensions;

namespace Framework.Interfaces.Repositories
{
    public interface IDapperRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        IEnumerable<TEntity> FetchAll(object predicate = null, IList<ISort> sort = null);
    }
}