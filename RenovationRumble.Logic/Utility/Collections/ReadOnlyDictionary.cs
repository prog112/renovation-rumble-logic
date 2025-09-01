namespace RenovationRumble.Logic.Utility.Collections
{
	using System.Collections;
	using System.Collections.Generic;

	public class ReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
	{
		public TValue this[TKey key] => dictionary[key];
		
		public int Count => dictionary.Count;
		public IEqualityComparer<TKey> Comparer => dictionary.Comparer;
		public Dictionary<TKey, TValue>.KeyCollection Keys => dictionary.Keys;
		public Dictionary<TKey, TValue>.ValueCollection Values => dictionary.Values;
		IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => dictionary.Keys;
		IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => dictionary.Values;

		private readonly Dictionary<TKey, TValue> dictionary;
		
		internal ReadOnlyDictionary(Dictionary<TKey, TValue> dictionary)
		{
			this.dictionary = dictionary;
		}

		public bool ContainsKey(TKey key)
		{
			return dictionary.ContainsKey(key);
		}
		
		public bool ContainsValue(TValue value)
		{
			return dictionary.ContainsValue(value);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return dictionary.TryGetValue(key, out value);
		}

		public TValue GetValueOrDefault(TKey key)
		{
			return dictionary.GetValueOrDefault(key);
		}
		
		public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}
	}
}