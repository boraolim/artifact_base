using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Hogar.Core.Shared.Extensions
{
    public static class ObjectExtensions
    {
        public static bool CheckIsNull(this object inputObject) => inputObject == null;

        public static List<TSource> ConvertTo<TSource, TDestination>(this List<TDestination> source, Func<TDestination, TSource> converter) =>
            source.Select(converter).ToList();

        public static bool ToBooleanSafe(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;

            string trimmed = value.Trim();
            return trimmed.Equals("true", StringComparison.OrdinalIgnoreCase)
                || trimmed.Equals("1", StringComparison.OrdinalIgnoreCase);
        }

        public static List<T> OrdenarLista<T>(this List<T> lista, string propiedad, bool orden)
        {
            var prop = typeof(T).GetProperty(propiedad);
            if (prop == null)
                throw new ArgumentException($"No se encontró la propiedad '{propiedad}' en el tipo '{typeof(T).Name}'.");

            return orden
                ? lista.OrderBy(x => prop.GetValue(x)).ToList()
                : lista.OrderByDescending(x => prop.GetValue(x)).ToList();
        }
    }
}
