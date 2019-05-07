using System;

namespace Framework.Interfaces.Factories
{
    public interface IGenericFactory<T> : IDisposable
        where T : class
    {
        T Create();

        void Release(T instance);
    }
}