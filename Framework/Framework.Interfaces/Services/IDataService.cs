namespace Framework.Interfaces.Services
{
    using System;

    public interface IDataService : IDisposable
    {
        void SaveChanges();
    }
}