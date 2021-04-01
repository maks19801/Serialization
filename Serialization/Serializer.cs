using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Serialization
{
    public static class Serializer
    {
        public static string serialized = string.Empty;

        public static Person deserializedObject = new Person();


        public static string Serialize(object @object)
        {
            if (@object == null)
            {
                return serialized.Substring(0, serialized.Length - 1);
            }
            var type = @object.GetType();
            var properties = type.GetProperties();
            serialized += $"{type.Name}*";
            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                if (!property.PropertyType.IsPrimitive && property.PropertyType.Name != "String")
                {
                    var propertyGetMethod = property.GetGetMethod();

                    if (propertyGetMethod == null) continue;
                    var propertyValue = propertyGetMethod.Invoke(@object, null);
                    if (propertyValue == null) break;
                    Serialize(propertyValue);
                }
                if (!property.PropertyType.IsPrimitive && property.PropertyType.Name != "String") break;
                var getMethod = property.GetGetMethod();

                if (getMethod == null) continue;
                serialized += $"{property.Name}:";
                var value = getMethod.Invoke(@object, null);
                serialized += value;
                serialized += "|";
            }
            return serialized.Substring(0, serialized.Length - 1);
        }



        public static T MakeAnObject<T>() where T : new()
        {
            var myObject = new T();

            return myObject;
        }


        public static T Deserialize1<T>(string serialized) where T : new()
        {
            if (serialized == null) throw new ArgumentNullException(nameof(serialized));
            var objects = serialized.Split("*");
            var deserializedObject = new T();
            var type = typeof(T);


            var typeName = objects[0];
            if (type.Name != typeName) throw new ArgumentException();

            var propertiesInfo = objects[1];

            var propertiesValues = propertiesInfo.Split("|");


            var properties = type.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                if (!property.PropertyType.IsPrimitive && property.PropertyType.Name != "String")
                {
                    var propertySetMethod = property.GetSetMethod();
                    if (propertySetMethod == null) continue;

                    var parameterType = propertySetMethod.GetParameters()[0].ParameterType;
                    MethodInfo method = typeof(Serializer).GetMethod(nameof(Serializer.MakeAnObject));
                    MethodInfo generic = method.MakeGenericMethod(parameterType);

                    var myObject = generic.Invoke(null, new object[] { });

                    var type1 = myObject.GetType();

                    propertySetMethod.Invoke(deserializedObject, new object[] { myObject });

                    var properties1 = type1.GetProperties();

                    var propertiesInfo1 = objects[2].Split("|");
                    for (int j = 0; j < propertiesInfo1.Length; j++)
                    {
                        var splitted2 = propertiesInfo1[j].Split(":");
                        var propertyName1 = splitted2[0];
                        var propertyValue = splitted2[1];
                        var setMethod1 = properties1
                        .First(p => p.Name == propertyName1)
                        .GetSetMethod();
                        if (setMethod1 == null) continue;

                        var parameterType1 = setMethod1.GetParameters()[0].ParameterType;
                        if (parameterType1 == typeof(int))
                        {
                            setMethod1.Invoke(myObject, new object[] { Convert.ToInt32(propertyValue) });
                        }
                        else if (parameterType1 == typeof(decimal))
                        {
                            setMethod1.Invoke(myObject, new object[] { Convert.ToDecimal(propertyValue) });
                        }
                        else if (parameterType1 == typeof(string))
                        {
                            setMethod1.Invoke(myObject, new[] { propertyValue });
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < propertiesValues.Length; j++)
                    {
                        var propertyValueInfo = propertiesValues[j];
                        var splitted = propertyValueInfo.Split(":");
                        if (splitted.Length < 2) break;
                        var propertyName = splitted[0];
                        var propertyValue = splitted[1];

                        var setMethod = properties
                            .First(p => p.Name == propertyName)
                            .GetSetMethod();

                        if (setMethod == null) continue;

                        var parameterType = setMethod.GetParameters()[0].ParameterType;
                        if (parameterType == typeof(int))
                        {
                            setMethod.Invoke(deserializedObject, new object[] { Convert.ToInt32(propertyValue) });
                        }
                        else if (parameterType == typeof(decimal))
                        {
                            setMethod.Invoke(deserializedObject, new object[] { Convert.ToDecimal(propertyValue) });
                        }
                        else if (parameterType == typeof(string))
                        {
                            setMethod.Invoke(deserializedObject, new[] { propertyValue });
                        }
                        else
                        {
                            throw new StatusCodeException(StatusCode.InvalidFunction, $"cannot parse {propertyName}");
                        }

                    }
                }
            }
            return deserializedObject;
        }
    }
}

