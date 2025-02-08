using Microsoft.Extensions.Configuration;
using Teste.Shared.Exceptions;

namespace Teste.Shared.Utilities;

/// <summary>
///     Helper methods for retrieving configuration values.
/// </summary>
public static class ConfigurationHelper
{
    /// <summary>
    ///     Retrieves a configuration value of the specified type <typeparamref name="T" /> from the given key.
    ///     Throws an exception if the value is not found or is invalid.
    /// </summary>
    /// <typeparam name="T">The expected type of the configuration value.</typeparam>
    /// <param name="configuration">The IConfiguration instance.</param>
    /// <param name="key">The key to retrieve the value from.</param>
    /// <returns>The configuration value of type <typeparamref name="T" />.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the key is missing, or the value is invalid or null.</exception>
    public static T GetConfiguration<T>(this IConfiguration configuration, string key)
    {
        if (configuration is null)
            throw new ConfigurationException(["Configuration instance cannot be null."]);

        if (string.IsNullOrWhiteSpace(key))
            throw new ConfigurationException(["Configuration key cannot be null or whitespace.", nameof(key)]);

        var value = configuration.GetSection(key).Get<T>();

        if (value is null || EqualityComparer<T>.Default.Equals(value, default!))
            throw new ConfigurationException(
            [
                $"The configuration key '{key}' is missing or cannot be converted to the expected type '{typeof(T).Name}'."
            ]);

        return value;
    }
}