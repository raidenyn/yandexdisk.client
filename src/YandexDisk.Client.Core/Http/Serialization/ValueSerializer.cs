using System;
using System.Globalization;
using System.Reflection;

namespace YandexDisk.Client.Http.Serialization
{
    internal class ValueSerializer : IObjectSerializer
    {
        public string Serialize(object obj, TypeInfo type)
        {
            if (type.IsEnum)
            {
                return SerializeEnum(obj, type);
            }
            if (type.AsType() == typeof(bool))
            {
                return obj.ToString().ToLower();
            }
            return Convert.ToString(obj, CultureInfo.InvariantCulture);
        }

        public string SerializeEnum(object obj, TypeInfo type)
        {
            string enumValue = Enum.GetName(type.AsType(), obj);

            return SnakeCasePropertyResolver.ToSnakeCase(enumValue);
        }
    }
}
