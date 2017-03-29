using System.Collections.Generic;
using System.Text;

namespace YandexDisk.Client.Http.Serialization
{
    internal abstract class PropertyResolver
    {
        public abstract string GetSerializedName(string propertyName);
    }

    internal class DefaultPropertyResolver : PropertyResolver
    {
        public override string GetSerializedName(string propertyName)
        {
            return propertyName;
        }
    }

    internal class CamelCasePropertyResolver : PropertyResolver
    {
        public override string GetSerializedName(string propertyName)
        {
            return ToCamelCase(propertyName);
        }

        /// <summary>
        /// From Newtonsoft.Json
        /// </summary>
        public static string ToCamelCase(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return propertyName;

            if (!char.IsUpper(propertyName[0]))
                return propertyName;

            char[] chars = propertyName.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                bool hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                    break;

                chars[i] = char.ToLower(chars[i]);
            }

            return new string(chars);
        }
    }

    internal class SnakeCasePropertyResolver : PropertyResolver
    {
        public override string GetSerializedName(string propertyName)
        {
            return ToSnakeCase(propertyName);
        }

        /// <summary>
        /// From https://gist.github.com/roryf/1042502
        /// </summary>
        public static string ToSnakeCase(string propertyName)
        {
            var parts = new List<string>();
            var currentWord = new StringBuilder();

            foreach (var c in propertyName)
            {
                if (char.IsUpper(c) && currentWord.Length > 0)
                {
                    parts.Add(currentWord.ToString());
                    currentWord.Clear();
                }
                currentWord.Append(char.ToLower(c));
            }

            if (currentWord.Length > 0)
            {
                parts.Add(currentWord.ToString());
            }

            return string.Join("_", parts.ToArray());
        }
    }
}
