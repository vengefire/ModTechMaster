namespace Framework.Utils.Extensions.Queryable
{
    using System.Collections.Generic;
    using System.Linq;

    public static class QueryableExtensions
    {
        public static IEnumerable<T> Paged<T>(
            this IQueryable<T> source,
            int page,
            int pageSize)
        {
            return source
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize);
        }
    }
}