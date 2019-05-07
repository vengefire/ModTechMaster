namespace Framework.Interfaces.Factories
{
    using System;
    using System.Data;

    public interface IDbConnectionFactory : IDisposable
    {
        IDbConnection Create();

        IDbConnection Create(string connectionString);

        void Release(IDbConnection connection);
    }
}