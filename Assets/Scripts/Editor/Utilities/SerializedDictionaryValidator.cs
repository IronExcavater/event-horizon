using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Editor.Utilities
{
    public class SerializedDictionaryValidator
    {
        private static readonly Dictionary<string, Type> _typeCache = new();

        public void ValidateEntry(SerializedProperty dictionary, SerializedProperty key, SerializedProperty value, out string message, out MessageType type)
        {
            message = null;
            type = MessageType.Error;

            // switch (dictionary.type)
            // {
            //
            // }
        }

        public static Type ResolveType(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;

            if (_typeCache.TryGetValue(fullName, out var cached))
                return cached;

            var type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); } catch { return Array.Empty<Type>(); }
                })
                .FirstOrDefault(t => t.FullName == fullName);

            _typeCache[fullName] = type;
            return type;
        }
    }
}
