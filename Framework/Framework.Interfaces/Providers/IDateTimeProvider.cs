using System;

namespace Framework.Interfaces.Providers
{
    /// <summary>
    ///     Provider for relative <see cref="DateTime" /> methods.
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        ///     Gets the current date/time.
        /// </summary>
        /// <value>
        ///     The current date/time.
        /// </value>
        DateTime Now { get; }

        /// <summary>
        ///     Gets today's <see cref="DateTime" />.
        /// </summary>
        /// <value>
        ///     Today <see cref="DateTime" />.
        /// </value>
        DateTime Today { get; }
    }
}