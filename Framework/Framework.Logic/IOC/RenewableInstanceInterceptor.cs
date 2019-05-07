namespace Framework.Logic.IOC
{
    using System;
    using System.Diagnostics;
    using Castle.DynamicProxy;

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
            this._builder = builder;
            this._expired = expired;
            this._cleanUp = cleanUp;
        }

        /// <summary>
        ///     Ensures that the child instance is disposed.
        /// </summary>
        public void Dispose()
        {
            this.DisposeInstance(this._instance);
            this._instance = null;
        }

        /// <summary>
        ///     Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            var localInstance = this._instance;
            if (localInstance == null ||
                this._expired(localInstance))
            {
                lock (this)
                {
                    if (this._instance == localInstance)
                    {
                        this.DisposeInstance(localInstance);

                        Trace.WriteLine(
                                        string.Format("Creating new instance of '{0}'.", typeof(T).FullName),
                                        this.GetType().Name);

                        this._instance = this._builder();
                        localInstance = this._instance;

                        Debug.Assert(localInstance != null, "instance cannot be null");
                    }
                    else
                    {
                        localInstance = this._instance;
                    }
                }
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            var target = (IChangeProxyTarget)invocation;
            target.ChangeInvocationTarget(localInstance);
            invocation.Proceed();
        }

        private void DisposeInstance(T localInstance)
        {
            if (localInstance == null)
            {
                return;
            }

            if (this._cleanUp != null)
            {
                this._cleanUp(localInstance);
            }

            var disposable = localInstance as IDisposable;
            if (disposable == null)
            {
                return;
            }

            Trace.WriteLine(string.Format("Disposing of old instance '{0}'.", typeof(T).FullName), this.GetType().Name);
            disposable.Dispose();
        }
    }
}