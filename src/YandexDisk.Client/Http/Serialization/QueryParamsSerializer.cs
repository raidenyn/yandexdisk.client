using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YandexDisk.Client.Http.Serialization
{
    /// <summary>
    /// Интерфейс настраиваемого сереализатора
    /// </summary>
    internal interface IObjectSerializer
    {
        string Serialize(object obj, TypeInfo type);
    }


    /// <summary>
    /// Сериализатор объекта в строку запроса в стиле MVC Framework
    /// </summary>
    internal class QueryParamsSerializer
    {
        public QueryParamsSerializer()
        {
            //Создаем общий серивализатор для всех простых типов
            DefaultSerializer = new ValueSerializer();

            //Сериализатор для строки
            RegisterSerializer(typeof(string).GetTypeInfo(), DefaultSerializer);
            //Сериализатор для времени
            RegisterSerializer(typeof(DateTime).GetTypeInfo(), new DateTimeSerializer());
            //Сериализатор для TimeSpan
            RegisterSerializer(typeof(TimeSpan).GetTypeInfo(), new FunctionSerializer((obj, type) =>
            {
                var timeSpan = (TimeSpan) obj;
                return $"{timeSpan.Days}.{timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}";
            }));
        }

        public PropertyResolver PropertyResolver { get; set; } = new SnakeCasePropertyResolver();

        #region Serialize
        public string Serialize<T>(T obj)
        {
            return Serialize(obj, typeof(T).GetTypeInfo());
        }

        public string Serialize<T>(T obj, string prefix)
        {
            return Serialize(obj, typeof(T).GetTypeInfo(), prefix);
        }

        public string Serialize(object obj)
        {
            if (obj == null)
            {
                return String.Empty;
            }
            return Serialize(obj, obj.GetType().GetTypeInfo());
        }

        public string Serialize(object obj, string prefix)
        {
            if (obj == null)
            {
                return String.Empty;
            }
            return Serialize(obj, obj.GetType().GetTypeInfo(), prefix);
        }

        public string Serialize(object obj, TypeInfo type)
        {
            return Serialize(obj, type, String.Empty);
        }

        public string Serialize(object obj, TypeInfo type, string prefix)
        {
            var stringParams = SerializeToDictionary(obj, type, prefix);

            return DictionarySerialize(stringParams);
        }

        public Dictionary<string, string> SerializeToDictionary(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            return SerializeToDictionary(obj, obj.GetType().GetTypeInfo(), String.Empty);
        }

        public Dictionary<string, string> SerializeToDictionary(object obj, TypeInfo type)
        {
            return SerializeToDictionary(obj, type, String.Empty);
        }

        public Dictionary<string, string> SerializeToDictionary(object obj, TypeInfo type, string prefix)
        {
            var parameters = new ParamBuilder(obj, type, prefix, _serializers.Keys);

            return ParameterSerialize(parameters);
        }

        private Dictionary<string, string> ParameterSerialize(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            var dic = new Dictionary<string, string>();

            foreach (var param in parameters.Where(param => param.Value != null))
            {
                var type = param.Value.GetType().GetTypeInfo();

                var value = Serialize(type, param.Value);

                dic.Add(PropertyResolver.GetSerializedName(param.Key), value);
            }

            return dic;
        }

        private string Serialize(TypeInfo type, object value)
        {
            if (type.IsArray)
            {
                var enumerableType = type.GetElementType().GetTypeInfo();
                return SerializeArray((IEnumerable)value, enumerableType);
            }

            return SerializeValue(type, value);
        }

        private string SerializeValue(TypeInfo type, object value)
        {
            IObjectSerializer serializer;

            if (!_serializers.TryGetValue(type, out serializer))
            {
                serializer = DefaultSerializer;
            }

            return serializer.Serialize(value, type);
        }

        private string SerializeArray(IEnumerable enumerable, TypeInfo enumerableType)
        {
            var values = from object item in enumerable select SerializeValue(enumerableType, item);

            return "\"" + String.Join(",", values) + "\"";
        }
        #endregion


        #region Custom Serializers
        private readonly Dictionary<TypeInfo, IObjectSerializer> _serializers = new Dictionary<TypeInfo, IObjectSerializer>();

        public void RegisterSerializer(TypeInfo type, IObjectSerializer serializer)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (_serializers.ContainsKey(type))
            {
                _serializers[type] = serializer;
            }
            else
            {
                _serializers.Add(type, serializer);
            }
        }

        public void RegisterSerializer(TypeInfo type, Func<object, TypeInfo, string> serializerFunc)
        {
            var serializer = new FunctionSerializer(serializerFunc);
            RegisterSerializer(type, serializer);
        }

        public void UnregisterSerializer(TypeInfo type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (_serializers.ContainsKey(type))
            {
                _serializers.Remove(type);
            }
        }

        public IObjectSerializer DefaultSerializer { get; set; }
        #endregion


        #region Concatenate List
        protected string DictionarySerialize(IEnumerable<KeyValuePair<string, string>> dic)
        {
            var parameters = from param in dic select
                $"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value)}";
            return String.Join("&", parameters);
        }
        #endregion


        #region Private Classes
        /// <summary>
        /// Строит список параметров по именам
        /// </summary>
        private class ParamBuilder : Dictionary<string, object>
        {
            /// <summary>
            /// Постройка дерева
            /// </summary>
            /// <param name="obj">Корневой объект для построения дерева</param>
            /// <param name="type">Тип объекта</param>
            /// <param name="prefix">Корневой префикс</param>
            /// <param name="simpleTypes">Типы, разбор членов которых не требуется</param>
            public ParamBuilder(object obj, TypeInfo type, string prefix, IEnumerable<TypeInfo> simpleTypes)
            {
                SimpleTypes = new HashSet<TypeInfo>(simpleTypes ?? new TypeInfo[0]);
                BuildParameters(prefix, obj, type);
            }

            /// <summary>
            /// Список типов не требующих сериализации
            /// </summary>
            public HashSet<TypeInfo> SimpleTypes { get; }

            /// <summary>
            /// Построение списка параметров
            /// </summary>
            private void BuildParameters(string prefix, object obj, TypeInfo type)
            {
                if (obj != null && !SimpleTypes.Contains(type))
                {
                    if (typeof(IDictionary).GetTypeInfo().IsAssignableFrom(type))
                    {
                        AddDictionary(prefix, obj);
                        return;
                    }

                    if (type.IsArray)
                    {
                        var genericType = type.GetElementType().GetTypeInfo();

                        if (genericType.IsValueType ||
                            SimpleTypes.Contains(genericType))
                        {
                            Add(prefix, obj);
                            return;
                        }
                    }

                    if (typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type))
                    {
                        AddEnumerable(prefix, obj);
                        return;
                    }

                    if (type.IsClass)
                    {
                        AddClass(prefix, obj, type);
                        return;
                    }
                }

                Add(prefix, obj);
            }


            private void AddClass(string prefix, object obj, TypeInfo type)
            {
                var properties = type.GetProperties();

                foreach (var property in properties)
                {
                    var name = property.Name;
                    if (!String.IsNullOrEmpty(prefix))
                    {
                        name = $"{prefix}.{name}";
                    }

                    BuildParameters(name, property.GetValue(obj, null), property.PropertyType.GetTypeInfo());
                }
            }

            private void AddEnumerable(string prefix, object obj)
            {
                var enumerable = (IEnumerable)obj;

                var counter = 0;
                foreach (var item in enumerable)
                {
                    var name = $"{prefix}[{counter}]";

                    BuildParameters(name, item, item?.GetType().GetTypeInfo() ?? typeof(object).GetTypeInfo());

                    counter++;
                }
            }

            private void AddDictionary(string prefix, object obj)
            {
                var dictionary = (IDictionary)obj;

                foreach (var itemKey in dictionary.Keys)
                {
                    var name = $"{prefix}[{itemKey}]";

                    var item = dictionary[itemKey];

                    BuildParameters(name, item, item?.GetType().GetTypeInfo() ?? typeof(object).GetTypeInfo());
                }
            }
        }

        /// <summary>
        /// Функциональный сериалайзер
        /// </summary>
        private class FunctionSerializer : IObjectSerializer
        {
            public FunctionSerializer(Func<object, TypeInfo, string> serializerFunc)
            {
                _serializerFunc = serializerFunc;
            }

            private readonly Func<object, TypeInfo, string> _serializerFunc;

            public string Serialize(object obj, TypeInfo type)
            {
                return _serializerFunc(obj, type);
            }
        }


        #endregion
    }
}
