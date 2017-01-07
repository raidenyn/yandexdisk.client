using System;
using System.Globalization;
using System.Reflection;

namespace YandexDisk.Client.Http.Serialization
{
    internal class DateTimeSerializer : IObjectSerializer
    {
        public string Serialize(object obj, TypeInfo type)
        {
            return ((DateTime)obj).ToString(Format, CultureInfo.InvariantCulture);
        }

        public string Format { get; set; } = @"dd.MM.yyyy'T'HH:mm:ss";
    }
}
