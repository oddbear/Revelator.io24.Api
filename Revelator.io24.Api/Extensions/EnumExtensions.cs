using System;
using System.ComponentModel;
using System.Reflection;

namespace Revelator.io24.Api.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription<TEnum>(this TEnum enumValue)
            where TEnum : Enum
        {
            var name = enumValue.ToString();

            var field = typeof(TEnum).GetField(name);
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            var description = attribute?.Description;

            return string.IsNullOrWhiteSpace(description)
                ? name
                : description;
        }

        public static TEnum ParseDescription<TEnum>(string enumDescription)
            where TEnum : Enum
        {
            var type = typeof(TEnum);
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
                var description = attribute?.Description;
                if (enumDescription == description)
                {
                    return (TEnum)Enum.Parse(type, field.Name);
                }
            }

            throw new InvalidOperationException();
        }
    }
}
