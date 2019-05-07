using System;
using System.Data;

namespace Framework.Interfaces.Factories
{
    public interface IDbConnectionFactory : IDisposable
    {
        IDbConnection Create();

        IDbConnection Create(string connectionString);

        void Release(IDbConnection connection);
    }
}