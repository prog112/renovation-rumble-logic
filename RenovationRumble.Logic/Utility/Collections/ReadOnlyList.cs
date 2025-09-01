namespace RenovationRumble.Logic.Utility.Collections
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class ReadOnlyList<T> : IReadOnlyList<T>
	{
		public T this[int index] => list[index];

		public int Count => list.Count;
		
		private readonly List<T> list;

		public ReadOnlyList(List<T> list)
		{
			this.list = list;
		}

		public bool Contains(T item)
		{
			return list.Contains(item);
		}

		public int BinarySearch(T item)
		{
			return list.BinarySearch(item);
		}

		public int BinarySearch(T item, IComparer<T> comparer)
		{
			return list.BinarySearch(item, comparer);
		}
		
		public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
		{
			return list.BinarySearch(index, count, item, comparer);
		}

		public int IndexOf(T item)
		{
			return list.IndexOf(item);
		}
		
		public int LastIndexOf(T item)
		{
			return list.LastIndexOf(item);
		}
		
		public int FindIndex(Predicate<T> match)
		{
			return list.FindIndex(match);
        }

        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return list.FindIndex(startIndex, match);
        }

        public int FindLastIndex(Predicate<T> match)
        {
            return list.FindLastIndex(match);
        }

        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return list.FindLastIndex(startIndex, match);
        }

        public T Find(Predicate<T> match)
		{
			return list.Find(match);
		}
		
		public T FindLast(Predicate<T> match)
		{
			return list.FindLast(match);
		}

		public List<T> FindAll(Predicate<T> match)
		{
			return list.FindAll(match);
		}

		public bool Exists(Predicate<T> match)
		{
			return list.Exists(match);
		}  

		public bool TrueForAll(Predicate<T> match)
		{
			return list.TrueForAll(match);
		}
		
		public void ForEach(Action<T> action)
		{
			list.ForEach(action);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
		{
			return list.ConvertAll(converter);
		}

		public List<T> GetRange(int index, int count)
		{
			return list.GetRange(index, count);
		}
		
		public T[] ToArray()
		{
			return list.ToArray();
		}

		public List<T>.Enumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}
	}
}