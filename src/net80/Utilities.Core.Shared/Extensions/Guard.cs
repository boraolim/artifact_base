namespace Utilities.Core.Shared.Extensions;

public static class Guard
{
    private static void ValidateParameterName(string parameterName)
    {
        if(string.IsNullOrWhiteSpace(parameterName))
            throw new ArgumentException("Parameter name cannot be null or empty.", nameof(parameterName));
    }

    private static string BuildMessage(string parameterName, string? message, string defaultMessage)
        => message ?? $"{defaultMessage} (Parameter: {parameterName})";

    private static void ThrowArgumentException(string parameterName, string? message, string defaultMessage)
    {
        ValidateParameterName(parameterName);

        throw new ArgumentException(BuildMessage(parameterName, message, defaultMessage), parameterName);
    }

    public static T AgainstNull<T>(T input, string parameterName, string? message = null) where T : class
    {
        ValidateParameterName(parameterName);

        if(input is null)
            throw new ArgumentNullException(parameterName, BuildMessage(parameterName, message, "Value cannot be null"));

        return input;
    }

    public static string AgainstNullOrEmpty(string input, string parameterName, string? message = null)
    {
        ValidateParameterName(parameterName);

        if(string.IsNullOrEmpty(input))
            ThrowArgumentException(parameterName, message, "Value cannot be null or empty");

        return input;
    }

    public static IEnumerable<T> AgainstNullOrEmpty<T>(IEnumerable<T>? input, string parameterName, string? message = null)
    {
        ValidateParameterName(parameterName);

        if(input is null || !input.Any())
            ThrowArgumentException(parameterName: parameterName, message: message, defaultMessage: "Collection cannot be null or empty");

        return input;
    }

    public static void AgainstNullOrWhiteSpace(string? input, string parameterName, string? message = null)
    {
        ValidateParameterName(parameterName);

        if(string.IsNullOrWhiteSpace(input))
            ThrowArgumentException(parameterName, message, "The value cannot be empty or whitespace");
    }

    public static T AgainstDefault<T>(T input, string parameterName, string? message = null)
    {
        ValidateParameterName(parameterName);

        if(EqualityComparer<T>.Default.Equals(input, default!))
            ThrowArgumentException(parameterName, message, "Value cannot be default");

        return input;
    }

    public static T Against<T>(T input, Func<T, bool> predicate, string parameterName, string? message = null)
    {
        ValidateParameterName(parameterName);

        AgainstNull(predicate, nameof(predicate));

        if(predicate(input))
            ThrowArgumentException(parameterName, message, "The value does not satisfy the required condition");

        return input;
    }

    private static void EnsureGreaterThanZero<T>(T input, string parameterName, string? message, string error)
        where T : struct, IComparable<T>
    {
        ValidateParameterName(parameterName);

        var zero = default(T);

        if(input.CompareTo(zero) <= 0)
            throw new ArgumentOutOfRangeException(parameterName, BuildMessage(parameterName, message, error));
    }

    public static T NegativeOrZero<T>(T input, string parameterName, string? message = null)
        where T : struct, IComparable<T>
    {
        EnsureGreaterThanZero(input, parameterName, message, "Value must be greater than zero");
        return input;
    }

    public static T Positive<T>(T input, string parameterName, string? message = null)
        where T : struct, IComparable<T>
    {
        EnsureGreaterThanZero(input, parameterName, message, "Value must be positive");
        return input;
    }

    public static bool False(bool input, string parameterName, string? message = null)
    {
        ValidateParameterName(parameterName);

        if(!input)
            ThrowArgumentException(parameterName, message, "Value must be true");

        return input;
    }

    public static bool True(bool input, string parameterName, string? message = null)
    {
        ValidateParameterName(parameterName);

        if(input)
            ThrowArgumentException(parameterName, message, "Value must be false");

        return input;
    }

    public static string MinLength(string input, int minLength, string parameterName, string? message = null)
    {
        AgainstNullOrEmpty(input, parameterName);

        if(input.Length < minLength)
            ThrowArgumentException(parameterName, message, $"Value must have at least {minLength} characters");

        return input;
    }

    public static string MaxLength(string input, int maxLength, string parameterName, string? message = null)
    {
        AgainstNullOrEmpty(input, parameterName);

        if(input.Length > maxLength)
            ThrowArgumentException(parameterName, message, $"Value must have no more than {maxLength} characters");

        return input;
    }

    public static T Range<T>(T input, T min, T max, string parameterName, string? message = null)
        where T : IComparable<T>
    {
        ValidateParameterName(parameterName);

        if(input.CompareTo(min) < 0 || input.CompareTo(max) > 0)
            throw new ArgumentOutOfRangeException(parameterName, BuildMessage(parameterName, message,
                $"Value must be between {min} and {max}"));

        return input;
    }

    public static Guid EmptyGuid(Guid input, string parameterName, string? message = null)
    {
        ValidateParameterName(parameterName);

        if(input == Guid.Empty)
            ThrowArgumentException(parameterName, message, "GUID cannot be empty");

        return input;
    }

    public static T EnumDefined<T>(T input, string parameterName, string? message = null) where T : struct, Enum
    {
        ValidateParameterName(parameterName);

        if(!Enum.IsDefined(typeof(T), input))
            ThrowArgumentException(parameterName, message, "Invalid enum value");

        return input;
    }

    public static T NotInList<T>(T input, IEnumerable<T> invalidValues, string parameterName, string? message = null)
    {
        ValidateParameterName(parameterName);
        AgainstNull(invalidValues, nameof(invalidValues));

        if(invalidValues.Contains(input))
            ThrowArgumentException(parameterName, message, "Value is forbidden");

        return input;
    }

    public static string StartsWith(string input, string prefix, string parameterName, string? message = null)
    {
        AgainstNullOrEmpty(input, parameterName);
        AgainstNullOrEmpty(prefix, nameof(prefix));

        if(!input.StartsWith(prefix))
            ThrowArgumentException(parameterName, message, $"Value must start with '{prefix}'");

        return input;
    }

    public static string EndsWith(string input, string suffix, string parameterName, string? message = null)
    {
        AgainstNullOrEmpty(input, parameterName);
        AgainstNullOrEmpty(suffix, nameof(suffix));

        if(!input.EndsWith(suffix))
            ThrowArgumentException(parameterName, message, $"Value must end with '{suffix}'");

        return input;
    }

    public static string RegexValue(string input, string pattern, string parameterName, string? message = null)
    {
        AgainstNullOrEmpty(input, parameterName);
        AgainstNullOrEmpty(pattern, nameof(pattern));

        if(!Regex.IsMatch(input, pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant, 
            TimeSpan.FromMilliseconds(MainConstantsCore.CFG_DEFAULT_REGEX_TIMEOUT)))
            throw new ArgumentException(BuildMessage(parameterName, message, "Value does not match required pattern"), parameterName);

        return input;
    }

    public static string JsonValid(string input, string parameterName, string? message = null)
    {
        AgainstNullOrEmpty(input, parameterName);

        try
        {
            System.Text.Json.JsonDocument.Parse(input);
        }
        catch
        {
            ThrowArgumentException(parameterName, message, "Invalid JSON");
        }

        return input;
    }

    public static string Url(string input, string parameterName, string? message = null)
    {
        AgainstNullOrEmpty(input, parameterName);

        if(!Uri.TryCreate(input, UriKind.Absolute, out _))
            ThrowArgumentException(parameterName, message, "Invalid URL");

        return input;
    }

    public static string FileExists(string path, string parameterName, string? message = null)
    {
        AgainstNullOrEmpty(path, parameterName);

        if(!File.Exists(path))
            throw new FileNotFoundException(message ?? $"File does not exist (Parameter: {parameterName})", path);

        return path;
    }

    public static string DirectoryExists(string path, string parameterName, string? message = null)
    {
        AgainstNullOrEmpty(path, parameterName);

        if(!Directory.Exists(path))
            throw new DirectoryNotFoundException(message ?? $"Directory does not exist (Parameter: {parameterName})");

        return path;
    }

    public static T NotEqual<T>(T input, T disallowed, string parameterName, string? message = null)
    {
        ValidateParameterName(parameterName);

        if(EqualityComparer<T>.Default.Equals(input, disallowed))
            ThrowArgumentException(parameterName, message, $"Value cannot be equal to '{disallowed}'");

        return input;
    }

    public static IEnumerable<T> All<T>(IEnumerable<T> input, Func<T, bool> predicate, string parameterName, string errorMessage)
    {
        AgainstNull(input, parameterName);
        AgainstNull(predicate, nameof(predicate));

        foreach(var item in input)
        {
            if(!predicate(item))
                throw new ArgumentException(errorMessage, parameterName);
        }

        return input;
    }

    public static void AgainstOutOfRange<T>(T value, T min, T max, string parameterName, string? message = null)
        where T : IComparable<T>
    {
        if(value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            throw new ArgumentOutOfRangeException(
                parameterName,
                value,
                BuildMessage(parameterName, message, $"The value must be between {min} and {max}"));
    }
}
