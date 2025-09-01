namespace RenovationRumble.Logic.Utility.Collections
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class ReadOnlyArray<T> : IReadOnlyList<T>
	{
		public struct Enumerator : IEnumerator<T>
		{
			public T Current
			{
				get
				{
					if (index >= array.Length)
						throw new InvalidOperationException("Iterated beyond end");
					return array[index];
				}
			}
			
			object IEnumerator.Current => Current;
			
			private readonly T[] array;
			
			private int index;

			internal Enumerator(T[] array)
			{
				this.array = array;
				index = -1;
			}

			public bool MoveNext()
			{
				var length = array.Length;
				if (index < length)
					++index;
				return index != length;
			}

			public void Reset()
			{
				index = -1;
			}

			public void Dispose()
			{
				
			}
		}
		
		public T this[int index] => array[index];
		
		public int Length => array.Length;
		int IReadOnlyCollection<T>.Count => array.Length;
		
		private readonly T[] array;

		internal ReadOnlyArray(T[] array)
		{
			this.array = array;
		}

		public bool Contains(T item)
		{
			return Array.IndexOf(array, item) != -1;
		}

		public int BinarySearch(T item)
		{
			return Array.BinarySearch(array, item);
		}
		
		public int BinarySearch(T item, IComparer<T> comparer)
		{
			return Array.BinarySearch(array, item, comparer);
		}

		public int BinarySearch(int index, int count, T item)
		{
			return Array.BinarySearch(array, index, count, item);
		}
		
		public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
		{
			return Array.BinarySearch(array, index, count, item, comparer);
		}

		public int IndexOf(T item)
		{
			return Array.IndexOf(array, item);
		}

		public int LastIndexOf(T item)
		{
			return Array.LastIndexOf(array, item);
		}

		public int FindIndex(Predicate<T> match)
		{
			return Array.FindIndex(array, match);
		}
		
		public int FindLastIndex(Predicate<T> match)
		{
			return Array.FindLastIndex(array, match);
		}
		
		public T Find(Predicate<T> match)
		{
			return Array.Find(array, match);
		}
		
		public T FindLast(Predicate<T> match)
		{
			return Array.FindLast(array, match);
		}
		
		public T[] FindAll(Predicate<T> match)
		{
			return Array.FindAll(array, match);
		}
		
		public bool Exists(Predicate<T> match)
		{
			return Array.Exists(array, match);
		}
		
		public bool TrueForAll(Predicate<T> match)
		{
			return Array.TrueForAll(array, match);
		}
		
		public void ForEach(Action<T> action)
		{
			Array.ForEach(array, action);
		}
		
		public void CopyTo(T[] array, int arrayIndex)
		{
			this.array.CopyTo(array, arrayIndex);
		}
		
		public TOutput[] ConvertAll<TOutput>(Converter<T, TOutput> converter)
		{
			return Array.ConvertAll(array, converter);
		}
		
		public Enumerator GetEnumerator()
		{
			return new Enumerator(array);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}