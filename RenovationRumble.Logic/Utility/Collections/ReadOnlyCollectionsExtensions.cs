namespace RenovationRumble.Logic.Utility.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public static class ReadOnlyCollectionsExtensions
    {
        private static readonly ConditionalWeakTable<object, object> _ReadOnlyArrays = new ConditionalWeakTable<object, object>();
        private static readonly ConditionalWeakTable<object, object> _ReadOnlyLists = new ConditionalWeakTable<object, object>();
        private static readonly ConditionalWeakTable<object, object> _ReadOnlyHashSets = new ConditionalWeakTable<object, object>();
        private static readonly ConditionalWeakTable<object, object> _ReadOnlyDictionaries = new ConditionalWeakTable<object, object>();

        public static bool IsNullOrEmpty<T>(this ReadOnlyList<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static ReadOnlyArray<T> AsReadOnlyArray<T>(this T[] array)
        {
            array ??= Array.Empty<T>();

            return (ReadOnlyArray<T>)_ReadOnlyArrays.GetValue(
                array,
                key => new ReadOnlyArray<T>((T[])key)
            );
        }

        public static ReadOnlyList<T> AsReadOnlyList<T>(this List<T> list)
        {
            return (ReadOnlyList<T>)_ReadOnlyLists.GetValue(
                list,
                key => new ReadOnlyList<T>((List<T>)key)
            );
        }

        public static ReadOnlyHashSet<T> AsReadOnlyHashSet<T>(this HashSet<T> hashSet)
        {
            return (ReadOnlyHashSet<T>)_ReadOnlyHashSets.GetValue(
                hashSet,
                key => new ReadOnlyHashSet<T>((HashSet<T>)key)
            );
        }

        public static ReadOnlyDictionary<TKey, TValue> AsReadOnlyDictionary<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary)
        {
            return (ReadOnlyDictionary<TKey, TValue>)_ReadOnlyDictionaries.GetValue(
                dictionary,
                key => new ReadOnlyDictionary<TKey, TValue>((Dictionary<TKey, TValue>)key)
            );
        }
    }
}