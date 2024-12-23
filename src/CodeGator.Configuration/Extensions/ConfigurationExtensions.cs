
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Configuration;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// This class utility contains extension methods related to the <see cref="IConfiguration"/>
/// type.
/// </summary>
public static partial class ConfigurationExtensions
{
    // *******************************************************************
    // Public methods.
    // *******************************************************************

    #region Public methods

    /// <summary>
    /// This method determines if the specified field is missing, or not.
    /// </summary>
    /// <param name="configuration">The configuration object to use for the
    /// operation.</param>
    /// <param name="fieldName">The field name to use for the operation.</param>
    /// <returns>True if the field is missing; False otherwise.</returns>
    public static bool FieldIsMissing(
        [NotNull] this IConfiguration configuration,
        [NotNull] string fieldName
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(fieldName, nameof(fieldName));

        var result = null == configuration[fieldName] &&
            null == configuration[$"{fieldName}:0"];

        return result;
    }

    // *******************************************************************

    /// <summary>
    /// This method determines if the specified field contains an array, or
    /// not.
    /// </summary>
    /// <param name="configuration">The configuration object to use for the
    /// operation.</param>
    /// <param name="fieldName">The field name to use for the operation.</param>
    /// <returns>True if the field contains an array; False otherwise.</returns>
    public static bool FieldIsArray(
        [NotNull] this IConfiguration configuration,
        [NotNull] string fieldName
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(fieldName, nameof(fieldName));

        var result = null != configuration[$"{fieldName}:0"];
        return result;
    }

    // *******************************************************************

    /// <summary>
    /// This method determines if the specified configuration section has
    /// any child nodes, or not.
    /// </summary>
    /// <param name="configuration">The configuration object to use for the
    /// operation.</param>
    /// <returns>True if the configuration section has child nodes; False 
    /// otherwise.</returns>
    public static bool HasChildren(
        [NotNull] this IConfiguration configuration
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration));

        var result = configuration.GetChildren().Any();
        return result;
    }

    // *******************************************************************

    /// <summary>
    /// This method returns the parent section of the specified configuration
    /// object. If there is no parent, the same section is returned.
    /// </summary>
    /// <param name="configuration">The configuration object to use for the
    /// operation.</param>
    /// <returns>The parent section for the specified <see cref="IConfiguration"/>
    /// object.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static IConfigurationSection? GetParentSection(
        [NotNull] this IConfiguration configuration
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration));

        var root = configuration.GetRoot();
        var path = configuration.GetPath();

        var parentPath = string.IsNullOrEmpty(path) || !path.Contains(':')
            ? path
            : path[..path.LastIndexOf(':')];

        var parentSection = string.IsNullOrEmpty(parentPath)
            ? configuration
            : root?.GetSection(parentPath);

        return parentSection as IConfigurationSection;
    }

    // *******************************************************************

    /// <summary>
    /// This method always returns the root section of the specified 
    /// configuration hierarchy.
    /// </summary>
    /// <param name="configuration">The configuration object to use for the
    /// operation.</param>
    /// <returns>The root section for the specified <see cref="IConfiguration"/>
    /// object.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static IConfiguration? GetRoot(
        [NotNull] this IConfiguration configuration
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration));

        // This is, admittedly, a hack. But, it's also the only practical
        //   way, that I know of, to get to the root of the configuration
        //   tree, from anywhere within that tree. If you know a better way
        //   then feel free to share with the rest of the class.

        var root = configuration.GetFieldValue(
            "_root",
            true
            ) as IConfiguration;

        return root;
    }

    // *******************************************************************

    /// <summary>
    /// This method returns the path for the specified <see cref="IConfiguration"/>
    /// object.
    /// </summary>
    /// <param name="configuration">The configuration object to use for the
    /// operation.</param>
    /// <returns>The path for the specified <see cref="IConfiguration"/>
    /// object.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static string GetPath(
        [NotNull] this IConfiguration configuration
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration));

        var section = configuration.GetSection("");
        var path = section.Path.TrimEnd(':');

        return path;
    }

    // *******************************************************************

    /// <summary>
    /// This method returns the value for the specified <see cref="IConfiguration"/>
    /// object.
    /// </summary>
    /// <param name="configuration">The configuration object to use for the
    /// operation.</param>
    /// <returns>The value for the specified <see cref="IConfiguration"/>
    /// object.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static string GetValue(
        [NotNull] this IConfiguration configuration
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration));

        var section = configuration.GetSection("");
        var value = section.Value;

        return value ?? "";
    }

    // *******************************************************************

    /// <summary>
    /// This method safely copies a value from the specified key of the 
    /// configuration to the property on the target oject using a LINQ
    /// expression to identify the property to change. If the configuration
    /// key is missing, or the value is NULL, then no copy operation is
    /// performed - unless the <paramref name="allowSetNulls"/> property is
    /// set to TRUE.
    /// </summary>
    /// <typeparam name="TObj">The target object type.</typeparam>
    /// <typeparam name="TProp">The target property type.</typeparam>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name = "target" > The target object to use for the operation.</param>
    /// <param name="selector">The LINQ expression to use for the operation.</param>
    /// <param name="key">The configuration key to use for the operation.</param>
    /// <param name="allowSetNulls">Indicates whether to allow a NULL value to be
    /// copied to the specified property on the target object.</param>
    /// <returns>The value of the <paramref name="configuration"/> parameter.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static IConfiguration SafeCopy<TObj, TProp>(
        [NotNull] this IConfiguration configuration,
        [NotNull] TObj target,
        [NotNull] Expression<Func<TObj, TProp>> selector,
        [NotNull] string key,
        bool allowSetNulls = false
        ) where TObj : class
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key))
            .ThrowIfNull(target, nameof(target))
            .ThrowIfNull(selector, nameof(selector));

        if (selector.Body is MemberExpression)
        {
            if ((selector.Body as MemberExpression)?.Member is PropertyInfo)
            {
                var pi = (selector.Body as MemberExpression)?.Member as PropertyInfo;
                var tProp = typeof(TProp);
                var value = configuration[key];

                if (tProp.IsValueType)
                {
                    if (tProp.IsEnum)
                    {
                        var eValue = Enum.Parse(tProp, value ?? "");
                        if (eValue != null || allowSetNulls)
                        {
                            pi?.SetValue(target, eValue);
                        }
                    }
                    else if (tProp == typeof(bool) || tProp == typeof(bool?))
                    {
                        if (bool.TryParse(value, out bool bValue))
                        {
                            pi?.SetValue(target, bValue);
                        }
                    }
                    else if (tProp == typeof(TimeSpan) || tProp == typeof(TimeSpan?))
                    {
                        if (TimeSpan.TryParse(value, out TimeSpan tsValue))
                        {
                            pi?.SetValue(target, tsValue);
                        }
                    }
                    else if (tProp == typeof(DateTime) || tProp == typeof(DateTime?))
                    {
                        if (DateTime.TryParse(value, out DateTime dtValue))
                        {
                            pi?.SetValue(target, dtValue);
                        }
                    }
                    else if (tProp == typeof(double) || tProp == typeof(double?))
                    {
                        if (double.TryParse(value, out double dValue))
                        {
                            pi?.SetValue(target, dValue);
                        }
                    }
                    else if (tProp == typeof(int) || tProp == typeof(int?))
                    {
                        if (int.TryParse(value, out int iValue))
                        {
                            pi?.SetValue(target, iValue);
                        }
                    }
                    else if (tProp == typeof(long) || tProp == typeof(long?))
                    {
                        if (long.TryParse(value, out long iValue))
                        {
                            pi?.SetValue(target, iValue);
                        }
                    }
                    else if (tProp != pi?.PropertyType)
                    {
#pragma warning disable CS8604 // Possible null reference argument.
                        var cnvValue = Convert.ChangeType(value, pi?.PropertyType);
#pragma warning restore CS8604 // Possible null reference argument.
                        if (cnvValue != null || allowSetNulls)
                        {
                            pi?.SetValue(target, cnvValue, null);
                        }
                    }
                }
                else
                {
                    if (value is not null || allowSetNulls)
                    {
                        pi?.SetValue(target, value, null);
                    }
                }
            }
        }

        return configuration;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a list of key-value-pairs and converts them to
    /// the specified type before returning.
    /// </summary>
    /// <typeparam name="T">The type to use for the operation.</typeparam>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to read from.</param>
    /// <param name="value">The list of values read from the key.</param>
    /// <returns>True if the setting was read and converted; false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsList<T>(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        out IEnumerable<T> value
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        value = [];

        var list = new List<T>();
        var index = 0;

        while (configuration.TryGetAs($"{key}:{index}", out T? item))
        {
#pragma warning disable CS8604 // Possible null reference argument.
            list.Add(item);
#pragma warning restore CS8604 // Possible null reference argument.

            index++;
        }

        value = list.ToArray();

        var result = list.Count > 0;
        return result;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as <typeparamref name="T"/> value. 
    /// </summary>
    /// <typeparam name="T">The type to use for the operation.</typeparam>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to read from.</param>
    /// <param name="value">The list of values read from the key.</param>
    /// <returns>True if the setting was read and converted; false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAs<T>(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        out T? value
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        var result = false;
        value = default;

        var setting = configuration[key];

        if (string.IsNullOrEmpty(setting))
        {
            return result;
        }

        if (typeof(T).IsValueType)
        {
            // If we get here then T is a value type so the parsing could be 
            //   tricky. As a result, we'll defer to the type's TryGetAs overload 
            //   (if there is one) and then convert the results (if any), to a T.

            if (typeof(string) == typeof(T))
            {
                value = (T)Convert.ChangeType(setting, typeof(T));
                result = !string.IsNullOrEmpty($"{value}");
            }
            else if (typeof(T) == typeof(bool))
            {
                result = configuration.TryGetAsBoolean(key, out var bValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(bValue, typeof(T));
                }
            }
            else if (typeof(T).IsEnum)
            {
                value = (T)Convert.ChangeType(Enum.Parse(typeof(T), setting), typeof(T));
                result = !EqualityComparer<T>.Default.Equals(value, default);
            }
            else if (typeof(T) == typeof(TimeSpan))
            {
                result = configuration.TryGetAsTimeSpan(key, out var tsValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(tsValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(DateTime))
            {
                result = configuration.TryGetAsDateTime(key, out var dtValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(dtValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(DateTimeOffset))
            {
                result = configuration.TryGetAsDateTimeOffset(key, out var dtoValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(dtoValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(char))
            {
                result = configuration.TryGetAsChar(key, out var cValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(cValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(byte))
            {
                result = configuration.TryGetAsByte(key, out var bValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(bValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(int))
            {
                result = configuration.TryGetAsInt(key, out var iValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(iValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(uint))
            {
                result = configuration.TryGetAsUInt(key, out var uValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(uValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(long))
            {
                result = configuration.TryGetAsLong(key, out var lValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(lValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(ulong))
            {
                result = configuration.TryGetAsULong(key, out var ulValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(ulValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(float))
            {
                result = configuration.TryGetAsFloat(key, out var fValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(fValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(double))
            {
                result = configuration.TryGetAsDouble(key, out var dValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(dValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(decimal))
            {
                result = configuration.TryGetAsDecimal(key, out var dValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(dValue, typeof(T));
                }
            }
            else if (typeof(T) == typeof(Guid))
            {
                result = configuration.TryGetAsGuid(key, out var gValue);
                if (result)
                {
                    value = (T)Convert.ChangeType(gValue, typeof(T));
                }
            }
            else
            {
                // If we get here then T is a kind of value type that we 
                //   don't know about, so, we'll use change type and hope 
                //   for the best.

                value = (T)Convert.ChangeType(setting, typeof(T));
                result = !EqualityComparer<T>.Default.Equals(value, default);
            }
        }
        else
        {
            // If we get here then T is a ref type so we only have to convert
            //   the value to T and we're done.
            value = (T)Convert.ChangeType(setting, typeof(T));
            result = !EqualityComparer<T>.Default.Equals(value, default);
        }

        return result;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a boolean value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a boolean value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsBoolean(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        out bool value
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        value = default;

        var setting = configuration[key];

        if (string.IsNullOrEmpty(setting))
        {
            return false;
        }

        if (bool.TryParse(setting, out value))
        {
            return true;
        }

        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a char value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a char value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsChar(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        out char value
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        value = default;

        var setting = configuration[key];

        if (string.IsNullOrEmpty(setting))
        {
            return false;
        }

        if (char.TryParse(setting, out value))
        {
            return true;
        }

        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a TimeSpan value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a TimeSpan value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsTimeSpan(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        out TimeSpan value
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        value = default;
        var setting = configuration[key];

        if (string.IsNullOrEmpty(setting))
        {
            return false;
        }

        if (TimeSpan.TryParse(setting, out value))
        {
            return true;
        }

        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a DateTime value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a DateTime value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsDateTime(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        out DateTime value
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        value = default;

        var setting = configuration[key];

        if (string.IsNullOrEmpty(setting))
        {
            return false;
        }

        if (DateTime.TryParse(setting, out value))
        {
            return true;
        }

        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a DateTimeOffset value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a DateTimeOffset value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsDateTimeOffset(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        out DateTimeOffset value
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        value = default;

        var setting = configuration[key];

        if (string.IsNullOrEmpty(setting))
        {
            return false;
        }

        if (DateTimeOffset.TryParse(setting, out value))
        {
            return true;
        }

        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as an int value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to an int value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsInt(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        out int value
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        value = default;

        var setting = configuration[key];

        if (string.IsNullOrEmpty(setting))
        {
            return false;
        }

        if (int.TryParse(setting, out value))
        {
            return true;
        }

        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a uint value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a uint value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsUInt(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        out uint value
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        value = default;

        var setting = configuration[key];

        if (string.IsNullOrEmpty(setting))
        {
            return false;
        }

        if (uint.TryParse(setting, out value))
        {
            return true;
        }

        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a long value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a long value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsLong(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        out long value
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        value = default;

        var setting = configuration[key];

        if (string.IsNullOrEmpty(setting))
        {
            return false;
        }

        if (long.TryParse(setting, out value))
        {
            return true;
        }

        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a ulong value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a ulong value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsULong(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        out ulong value
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        value = default;

        var setting = configuration[key];

        if (string.IsNullOrEmpty(setting))
        {
            return false;
        }

        if (ulong.TryParse(setting, out value))
        {
            return true;
        }

        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a byte value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a byte value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsByte(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        out byte value
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        value = default;

        var setting = configuration[key];

        if (string.IsNullOrEmpty(setting))
        {
            return false;
        }

        if (byte.TryParse(setting, out value))
        {
            return true;
        }

        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a float value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a float value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsFloat(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        out float value
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        value = default;

        var setting = configuration[key];

        if (string.IsNullOrEmpty(setting))
        {
            return false;
        }

        if (float.TryParse(setting, out value))
        {
            return true;
        }

        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a double value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a double value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsDouble(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        out double value
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        value = default;

        var setting = configuration[key];

        if (string.IsNullOrEmpty(setting))
        {
            return false;
        }

        if (double.TryParse(setting, out value))
        {
            return true;
        }

        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a decimal value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a decimal value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsDecimal(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        out decimal value
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        value = default;

        var setting = configuration[key];

        if (string.IsNullOrEmpty(setting))
        {
            return false;
        }

        if (decimal.TryParse(setting, out value))
        {
            return true;
        }

        return false;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value as a GUID value. 
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="value">The value read by the operation.</param>
    /// <returns>True is the setting was read and converted to a GUID value; 
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool TryGetAsGuid(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        out Guid value
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        value = default;

        var setting = configuration[key];

        if (string.IsNullOrEmpty(setting))
        {
            return false;
        }

        var result = Guid.TryParse(setting, out var gValue);

        if (result)
        {
            value = (Guid)Convert.ChangeType(gValue, typeof(Guid));
        }

        return result;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static bool GetAsBoolean(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        bool defaultValue
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        if (!configuration.TryGetAsBoolean(key, out var retValue))
        {
            retValue = defaultValue;
        }

        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static int GetAsInt(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        int defaultValue
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        if (!configuration.TryGetAsInt(key, out var retValue))
        {
            retValue = defaultValue;
        }

        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static uint GetAsUInt(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        uint defaultValue
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        if (!configuration.TryGetAsUInt(key, out var retValue))
        {
            retValue = defaultValue;
        }

        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static long GetAsLong(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        long defaultValue
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        if (!configuration.TryGetAsLong(key, out var retValue))
        {
            retValue = defaultValue;
        }

        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static ulong GetAsULong(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        ulong defaultValue
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        if (!configuration.TryGetAsULong(key, out var retValue))
        {
            retValue = defaultValue;
        }

        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static byte GetAsByte(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        byte defaultValue
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        if (!configuration.TryGetAsByte(key, out var retValue))
        {
            retValue = defaultValue;
        }

        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static Guid GetAsGuid(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        Guid defaultValue
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        if (!configuration.TryGetAsGuid(key, out var retValue))
        {
            retValue = defaultValue;
        }

        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static TimeSpan GetAsTimeSpan(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        TimeSpan defaultValue
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        if (!configuration.TryGetAsTimeSpan(key, out var retValue))
        {
            retValue = defaultValue;
        }

        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static DateTime GetAsDateTime(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        DateTime defaultValue
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        if (!configuration.TryGetAsDateTime(key, out var retValue))
        {
            retValue = defaultValue;
        }

        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static DateTimeOffset GetAsDateTimeOffset(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        DateTimeOffset defaultValue
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        if (!configuration.TryGetAsDateTimeOffset(key, out var retValue))
        {
            retValue = defaultValue;
        }

        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static decimal GetAsDecimal(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        decimal defaultValue
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        if (!configuration.TryGetAsDecimal(key, out var retValue))
        {
            retValue = defaultValue;
        }

        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method reads a key-value pair from an <see cref="IConfiguration"/> 
    /// object, parses it, and returns the value, or a default if the key was
    /// missing or the value couldn't be parsed.
    /// </summary>
    /// <typeparam name="T">The type to use for the operation.</typeparam>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="key">The key to use for the operation.</param>
    /// <param name="defaultValue">The value to return if the key is missing 
    /// or the value can't be parsed.</param>
    /// <returns>The value associated with the specified key, in the configuration,
    /// or the default value if the key is missing, or can't be parsed into 
    /// the desired type.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the argument is missing, or invalid.</exception>
    public static T? GetAs<T>(
        [NotNull] this IConfiguration configuration,
        [NotNull] string key,
        T defaultValue
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(key, nameof(key));

        if (!configuration.TryGetAs(key, out T? retValue))
        {
            retValue = defaultValue;
        }

        return retValue;
    }

    // *******************************************************************

    /// <summary>
    /// This method converts the given configuration object into equivalent 
    /// JSON text.
    /// </summary>
    /// <param name="configuration">The configuration to use for the operation.</param>
    /// <param name="level">The level for the operation.</param>
    /// <returns>Formatted JSON text.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever 
    /// one or more arguments is missing or invalid.</exception>
    public static string ToJson(
        [NotNull] this IConfiguration configuration,
        int level = 1
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfLessThanOrEqualZero(level, nameof(level));

        var sb = new StringBuilder("{" + Environment.NewLine);
        var tabs = new string('\t', level);

        foreach (var child in configuration.GetChildren())
        {
            if (child.HasChildren())
            {
                sb.AppendLine($"{tabs}\"{child.Key}\": {child.ToJson(level + 1)},");
            }
            else
            {
                sb.AppendLine($"{tabs}\"{child.Key}\": \"{child.Value}\",");
            }
        }

        sb.Remove(sb.Length - 1, 1);
        sb.AppendLine("}" + Environment.NewLine);

        return sb.ToString();
    }

    // *******************************************************************

    /// <summary>
    /// This method blasts the contents of the given configuration object
    /// to the specified file, as formatted JSON text.  
    /// </summary>
    /// <param name="configuration">The configuration object to use for 
    /// the operation.</param>
    /// <param name="filePath">The file to write to, or create if needed.</param>
    /// <exception cref="ArgumentException">This exception is thrown whenever 
    /// one or more arguments is missing or invalid.</exception>
    /// <remarks>
    /// <para>
    /// The file is created if needed. The contents of the file are overwritten 
    /// without warning. The contents of the configuration object are not 
    /// distributed between environments.
    /// </para>
    /// </remarks>
    public static void WriteAsJSON(
        [NotNull] this IConfiguration configuration,
        [NotNull] string filePath
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(filePath, nameof(filePath));

        var json = configuration.ToJson();

        File.WriteAllText(filePath, json);
    }

    // *******************************************************************

    /// <summary>
    /// This method blasts the contents of the given configuration object
    /// to the specified file, as formatted JSON text.  
    /// </summary>
    /// <param name="configuration">The configuration object to use for 
    /// the operation.</param>
    /// <param name="filePath">The file to write to, or create if needed.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task to perform the operation.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever 
    /// one or more arguments is missing or invalid.</exception>
    /// <remarks>
    /// <para>
    /// The file is created if needed. The contents of the file are overwritten 
    /// without warning. The contents of the configuration object are not 
    /// distributed between environments.
    /// </para>
    /// </remarks>
    public static async Task WriteAsJSONAsync(
        [NotNull] this IConfiguration configuration,
        [NotNull] string filePath,
        CancellationToken cancellationToken = default
        )
    {
        Guard.Instance().ThrowIfNull(configuration, nameof(configuration))
            .ThrowIfNullOrEmpty(filePath, nameof(filePath));

        var json = configuration.ToJson();

        await File.WriteAllTextAsync(
            filePath,
            json,
            cancellationToken
            ).ConfigureAwait(false);
    }

    #endregion

    // *******************************************************************
    // Private methods.
    // *******************************************************************

    #region Private methods

    /// <summary>
    /// This method reads a field value from the specified object.
    /// </summary>
    /// <param name="obj">The object to use for the operation.</param>
    /// <param name="fieldName">The field to use for the operation.</param>
    /// <param name="includeProtected">Determines if protected fields are included 
    /// in the search.</param>
    /// <returns>The value of the field.</returns>
    /// <remarks>
    /// <para>The idea, with this method, is to use reflection to go find
    /// and return a field value from an object at runtime. The intent is 
    /// to use this sparingly because the performance isn't great. I see
    /// this approach as something useful for things like unit testing.</para>
    /// </remarks>
    private static object? GetFieldValue(
        [NotNull] this object obj,
        [NotNull] string fieldName,
        bool includeProtected = false
        )
    {
        Guard.Instance().ThrowIfNull(obj, nameof(obj))
            .ThrowIfNullOrEmpty(fieldName, nameof(fieldName));

        var type = obj.GetType();

        var fi = type.GetField(
            fieldName,
            BindingFlags.Static |
            BindingFlags.Instance |
            BindingFlags.Public |
            (includeProtected ? BindingFlags.NonPublic : BindingFlags.Public)
            );

        if (fi == null)
        {
            return null;
        }

        var value = fi.GetValue(obj);
        var valType = value?.GetType();

        if (valType == typeof(WeakReference))
        {
            return (value as WeakReference)?.Target;
        }

        return value;
    }

    #endregion
}
