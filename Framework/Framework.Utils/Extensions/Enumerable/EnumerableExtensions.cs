namespace Framework.Utils.Extensions.Enumerable
{
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Paged<T>(
            this IEnumerable<T> source,
            int page,
            int pageSize)
        {
            return source
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize);
        }
    }
}