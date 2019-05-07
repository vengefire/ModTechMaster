namespace Framework.Interfaces.Factories
{
    using System;

    public interface IGenericFactory<T> : IDisposable
        where T : class
    {
        T Create();

        void Release(T instance);
    }
}