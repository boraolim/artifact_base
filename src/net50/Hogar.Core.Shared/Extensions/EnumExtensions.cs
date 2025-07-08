using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Bankaool.Core.Shared.Extensions
{
    public static class EnumExtensions
    {

        public static T GetAttribute<T>(this System.Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0 ? (T)attributes[0] : null;
        }

        public static string GetDescription(this System.Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute.CheckIsNull() ? value.ToString() : attribute.Description;
        }

        public static Type GetUnderlyingType<T>(this T enumValue) where T : System.Enum =>
            System.Enum.GetUnderlyingType(typeof(T));
        public static T Parse<T>(this string value) where T : System.Enum =>
            (T)System.Enum.Parse(typeof(T), value);
        public static string GetName<T>(this T enumValue) where T : System. Enum =>
            System.Enum.GetName(typeof(T), enumValue);
        public static List<T> GetValues<T>() where T : System.Enum =>
            System.Enum.GetValues(typeof(T)).Cast<T>().ToList();
        public static bool IsDefined<T>(this T enumValue) where T : System.Enum =>
            System.Enum.IsDefined(typeof(T), enumValue);
        public static string ToStringValue<T>(this T enumValue) where T : System.Enum =>
            enumValue.ToString();
        public static string ToStringLowerValue<T>(this T enumValue) where T : System.Enum =>
            enumValue.ToString().ToLower();
        public static string ToStringUpperValue<T>(this T enumValue) where T : System.Enum =>
            enumValue.ToString().ToUpper();

        public static T GetRandomValue<T>() where T : System.Enum
        {
            var values = Enum.GetValues(typeof(T));
            var rnd = RandomNumberGenerator.GetInt32(0, values.Length);
            return (T)values.GetValue(rnd)!;
        }

        public static TEnum? ToNullable<TEnum>(this TEnum enumValue) where TEnum : struct, System.Enum => null;
    }
}
