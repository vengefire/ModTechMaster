using System;
using System.Diagnostics;
using Castle.DynamicProxy;

namespace Framework.Logic.IOC
{
    /// <summary>
    ///     Interceptor to make a target fault tolerant.
    /// </summary>
    /// <typeparam name="T">Target instance.</typeparam>
    public class RenewableInstanceInterceptor<T> : IInterceptor, IDisposable
        where T : class
    {
        private readonly Func<T> _builder;

        private readonly Action<T> _cleanUp;

        private readonly Func<T, bool> _expired;

        private T _instance;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RenewableInstanceInterceptor{T}" /> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="expired">The expired.</param>
        /// <param name="cleanUp">Clean up action to execute against old object before disposal.</param>
        public RenewableInstanceInterceptor(Func<T> builder, Func<T, bool> expired, Action<T> cleanUp = null)
        {
            _builder = builder;
            _expired = expired;
            _cleanUp = cleanUp;
        }

        /// <summary>
        ///     Ensures that the child instance is disposed.
        /// </summary>
        public void Dispose()
        {
            DisposeInstance(_instance);
            _instance = null;
        }

        /// <summary>
        ///     Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            var localInstance = _instance;
            if ((localInstance == null) || _expired(localInstance))
            {
                lock (this)
                {
                    if (_instance == localInstance)
                    {
                        DisposeInstance(localInstance);

                        Trace.WriteLine(string.Format("Creating new instance of '{0}'.", typeof(T).FullName),
                            GetType().Name);

                        _instance = _builder();
                        localInstance = _instance;

                        Debug.Assert(localInstance != null, "instance cannot be null");
                    }
                    else
                    {
                        localInstance = _instance;
                    }
                }
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            var target = (IChangeProxyTarget) invocation;
            target.ChangeInvocationTarget(localInstance);
            invocation.Proceed();
        }

        private void DisposeInstance(T localInstance)
        {
            if (localInstance == null)
            {
                return;
            }

            if (_cleanUp != null)
            {
                _cleanUp(localInstance);
            }

            var disposable = localInstance as IDisposable;
            if (disposable == null)
            {
                return;
            }

            Trace.WriteLine(string.Format("Disposing of old instance '{0}'.", typeof(T).FullName), GetType().Name);
            disposable.Dispose();
        }
    }
}