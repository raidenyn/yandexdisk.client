using System;
using System.Globalization;

namespace YandexDisk.Client.Http.Serialization
{
    internal class ValueSerializer : IObjectSerializer
    {
        public string Serialize(object obj, Type type)
        {
            if (type.IsEnum)
            {
                return SerializeEnum(obj, type);
            }
            if (type == typeof(bool))
            {
                return obj.ToString().ToLower();
            }
            return Convert.ToString(obj, CultureInfo.InvariantCulture);
        }

        public string SerializeEnum(object obj, Type type)
        {
            string enumValue = Enum.GetName(type, obj);

            return SnakeCasePropertyResolver.ToSnakeCase(enumValue);
        }
    }
}
