
using System;
using System.Collections.Generic;
using System.Linq;

namespace StateVisualController.Util
{
    public static class ReflectUtil
    {
        public static Type[] GetAllImplementTypes<T>()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(T).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
                .ToArray();
        }

        public static T GetAttribute<T>(Type type) where T : Attribute
        {
            var attributes = type.GetCustomAttributes(typeof(T), false);
            if (attributes.Length == 0) return null;

            return attributes[0] as T;
        }

        public static IReadOnlyList<T> GetFields<T>(Type type)
        {
            return type.GetFields()
                .Where(x => x.FieldType == typeof(T))
                .Select(x => (T)x.GetValue(null))
                .ToList();
        }
    }
}