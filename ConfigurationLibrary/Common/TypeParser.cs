using ConfigurationLibrary.Interfaces;
using System;

namespace ConfigurationLibrary.Common
{
    public class TypeParser : ITypeParser
    {
        public T Parse<T>(string value)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Int32:
                    if (int.TryParse(value, out var intValue))
                        return (T)(object)intValue;
                    break;

                case TypeCode.Double:
                    if (double.TryParse(value, out var doubleValue))
                        return (T)(object)doubleValue;
                    break;

                case TypeCode.Boolean:
                    if (bool.TryParse(value, out var boolValue))
                        return (T)(object)boolValue;
                    break;

                case TypeCode.String:
                    return (T)(object)value;

                default:
                    throw new InvalidCastException($"Unsupported type: {typeof(T)}");
            }

            throw new InvalidCastException($"Unable to parse '{value}' to type {typeof(T)}");
        }
    }
}
