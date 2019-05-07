using System;

namespace Framework.Interfaces.Services
{
    public interface IDataService : IDisposable
    {
        void SaveChanges();
    }
}