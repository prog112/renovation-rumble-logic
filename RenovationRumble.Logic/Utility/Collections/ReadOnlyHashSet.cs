namespace RenovationRumble.Logic.Utility.Collections
{
	using System.Collections;
	using System.Collections.Generic;

	public class ReadOnlyHashSet<T> : IReadOnlyCollection<T>
	{
		public int Count => hashSet.Count;
		public IEqualityComparer<T> Comparer => hashSet.Comparer;
		
		private readonly HashSet<T> hashSet;

		internal ReadOnlyHashSet(HashSet<T> hashSet)
		{
			this.hashSet = hashSet;
		}

		public bool Contains(T item)
		{
			return hashSet.Contains(item);
		}

		public bool TryGetValue(T equalValue, out T actualValue)
		{
			return hashSet.TryGetValue(equalValue, out actualValue);
		}

		public bool SetEquals(IEnumerable<T> other)
		{
			return hashSet.SetEquals(other);
		}
		
		public bool Overlaps(IEnumerable<T> other)
		{
			return hashSet.Overlaps(other);
		}
		
		public bool IsSubsetOf(IEnumerable<T> other)
		{
			return hashSet.IsSubsetOf(other);
		}
		
		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			return hashSet.IsProperSubsetOf(other);
		}
		
		public bool IsSupersetOf(IEnumerable<T> other)
		{
			return hashSet.IsSupersetOf(other);
		}
		
		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			return hashSet.IsProperSupersetOf(other);
		}
		
		public void CopyTo(T[] array)
		{
			hashSet.CopyTo(array);
		}
		
		public void CopyTo(T[] array, int arrayIndex)
		{
			hashSet.CopyTo(array, arrayIndex);
		}
		
		public void CopyTo(T[] array, int arrayIndex, int count)
		{
			hashSet.CopyTo(array, arrayIndex, count);
		}
		
		public HashSet<T>.Enumerator GetEnumerator()
		{
			return hashSet.GetEnumerator();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return hashSet.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return hashSet.GetEnumerator();
		}
	}
}