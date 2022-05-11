using BackendOperacionesFroward.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BackendOperacionesFroward
{
    public class Caster
    {
        public static object CastPropertyValue(PropertyInfo property, Object value)
        {
            if (property == null || String.IsNullOrEmpty(value.ToString()))
                return null;

            else if (property.PropertyType == typeof(string))
                return $"{value}";

            else if (property.PropertyType == typeof(bool?))
                return bool.Parse(value.ToString());

            if (property.PropertyType.IsEnum)
            {
                Type enumType = property.PropertyType;
                if (Enum.IsDefined(enumType, value))
                    return Enum.Parse(enumType, value.ToString());
            }

            if (property.PropertyType == typeof(bool))
                return value == "1" || value == "true" || value == "on" || value == "checked";

            else if (property.PropertyType == typeof(DateTime?))
                return DateTime.Parse(value.ToString());


            else if (property.PropertyType == typeof(int?))
                return Int32.Parse(value.ToString());

            else
                return Convert.ChangeType(value, property.PropertyType);
        }
    }
}
