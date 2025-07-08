using System;
using System.Linq;
using System.Collections.Generic;

namespace Bankaool.Core.Shared.Extensions
{
    public static class Guard
    {
        public static T AgainstNull<T>(T input, string parameterName) where T : class
        {
            if (input is null)
                throw new ArgumentNullException(parameterName);

            return input;
        }

        public static string AgainstNullOrEmpty(string input, string parameterName)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException($"'{parameterName}' cannot be null or empty.", parameterName);

            return input;
        }

        public static IEnumerable<T> AgainstNullOrEmpty<T>(IEnumerable<T> input, string parameterName)
        {
            if (input.CheckIsNull() || !input.Any())
                throw new ArgumentException($"'{parameterName}' cannot be null or empty.", parameterName);

            return input;
        }

        public static T AgainstDefault<T>(T input, string parameterName)
        {
            if (EqualityComparer<T>.Default.Equals(input, default!))
                throw new ArgumentException($"'{parameterName}' cannot be default value.", parameterName);

            return input;
        }

        public static T Against<T>(T input, Func<T, bool> predicate, string message, string parameterName)
        {
            if (predicate(input))
                throw new ArgumentException(message, parameterName);

            return input;
        }

        public static T NegativeOrZero<T>(T input, string parameterName) where T : struct, IComparable<T>
        {
            var zero = default(T);
            if (input.CompareTo(zero) <= 0)
                throw new ArgumentOutOfRangeException(parameterName, $"'{parameterName}' must be greater than zero.");

            return input;
        }
    }
}
