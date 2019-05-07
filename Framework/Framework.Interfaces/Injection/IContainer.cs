namespace Framework.Interfaces.Injection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///     IOC container.
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        ///     Determines if the configuration is valid.
        /// </summary>
        /// <param name="details">Details of misconfiguration.</param>
        /// <returns><c>True</c> if configuration is valid; otherwise <c>false</c>.</returns>
        bool ConfigurationIsValid(out string details);

        /// <summary>
        ///     Determines if the configuration for the specified type is valid.
        /// </summary>
        /// <param name="type">Type to confirm.</param>
        /// <param name="details">Details of misconfiguration.</param>
        /// <returns><c>True</c> if configuration is valid; otherwise <c>false</c>.</returns>
        bool ConfigurationIsValid(Type type, out string details);

        /// <summary>
        ///     Gets an instance of the type specified by T.
        /// </summary>
        /// <typeparam name="T">The type to get.</typeparam>
        /// <returns>Instance of T.</returns>
        T GetInstance<T>();

        /// <summary>
        ///     Gets an instance of the type specified by T.
        /// </summary>
        /// ///
        /// <param name="args">Dictionary of arguments.</param>
        /// <typeparam name="T">The type to get.</typeparam>
        /// <returns>Instance of T.</returns>
        T GetInstance<T>(IDictionary args);

        /// <summary>
        ///     Gets an instance of the type specified by T.
        /// </summary>
        /// <param name="name">Type registration's name.</param>
        /// <typeparam name="T">The type to get.</typeparam>
        /// <returns>Instance of T.</returns>
        T GetInstance<T>(string name);

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Instance of the specified type.</returns>
        object GetInstance(Type type);

        /// <summary>
        ///     Gets all instances of the specified type.
        /// </summary>
        /// <typeparam name="T">Types to retrieve.</typeparam>
        /// <returns>Instances of the specified type.</returns>
        IEnumerable<T> GetAllInstances<T>();

        /// <summary>
        ///     Gets all instances of the specified type.
        /// </summary>
        /// <returns>Instances of the specified type.</returns>
        IEnumerable GetAllInstances(Type type);

        /// <summary>
        ///     Release object from container.
        /// </summary>
        /// <param name="instance">Object to release.</param>
        void Release(object instance);
    }
}