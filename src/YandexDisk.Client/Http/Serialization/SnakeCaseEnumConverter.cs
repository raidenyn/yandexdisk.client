using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace YandexDisk.Client.Http.Serialization
{
    internal class SnakeCaseEnumConverter: JsonConverter
    {
        private readonly StringEnumConverter _stringEnumConverter = new StringEnumConverter();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            Enum e = (Enum)value;

            string enumName = e.ToString("G");

            if (char.IsNumber(enumName[0]) || 
                enumName[0] == '-')
            {
                // enum value has no name so write number
                writer.WriteValue(value);
            }
            else
            {
                string finalName = SnakeCasePropertyResolver.ToSnakeCase(enumName);

                _stringEnumConverter.WriteJson(writer, finalName, serializer);
            }
        }

        class OneStringJsonReader: JsonReader
        {
            public OneStringJsonReader(string value)
            {
                Value = value;
            }

            public override bool Read()
            {
                throw new NotImplementedException("This method should not be called.");
            }

            public override JsonToken TokenType { get; } = JsonToken.String;

            public override object Value { get; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                string enumText = reader.Value.ToString();

                enumText = enumText.Replace("-", "");

                reader = new OneStringJsonReader(enumText);
            }

            return _stringEnumConverter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override bool CanConvert(Type objectType)
        {
            return _stringEnumConverter.CanConvert(objectType);
        }
    }
}
