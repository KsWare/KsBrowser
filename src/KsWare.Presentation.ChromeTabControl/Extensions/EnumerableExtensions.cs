using System;
using System.Collections;
using System.Linq;
using System.Windows;
using LinqEnumerable = System.Linq.Enumerable;

namespace KsWare.Presentation.Extensions {

	public static class EnumerableExtensions {
		/*
		 * object FirstOrDefault()
		 * object FirstOrDefault(predict)	=> Linq OfType<object>
		 * object First()
		 * object First(predict)			=> Linq OfType<object>
		 * T FirstOrDefault<T>()
		 * T FirstOrDefault<T>(predict)		=> Linq OfType<T>
		 * T First<T>()
		 * T First<T>(predict)				=> Linq OfType<T>
		 *
		 * object LastOrDefault()
		 * object LastOrDefault(predict)	=> Linq OfType<object>
		 * object Last()
		 * object Last(predict)				=> Linq OfType<object>
		 * T LastOrDefault<T>()
		 * T LastOrDefault<T>(predict)		=> Linq OfType<T>
		 * T Last<T>()
		 * T Last<T>(predict)				=> Linq OfType<T>
		 *
		 * Count()
		 *
		 *
		 */

		private static void _() {
			// LinqEnumerable.Any(Array.Empty<object>());
			// LinqEnumerable.Aggregate(Array.Empty<object>());
			// LinqEnumerable.All(Array.Empty<object>());
			// LinqEnumerable.Append(Array.Empty<object>());
			// LinqEnumerable.AsEnumerable(Array.Empty<object>());
			// LinqEnumerable.Average(Array.Empty<object>());
			// ...
			// LinqEnumerable.Cast<object>(Array.Empty<object>());
			// LinqEnumerable.Count(Array.Empty<object>());
		}

		public static object FirstOrDefault(this IEnumerable enumerable) {
			if (enumerable is IList list) return list.Count > 0 ? list[0] : null;
			var enumerator = enumerable.GetEnumerator();
			if (!enumerator.MoveNext()) return null;
			var value = enumerator.Current;
			return value;
		}

		public static object FirstOrDefault(this IEnumerable enumerable, Func<object, bool> predict) =>
			System.Linq.Enumerable.FirstOrDefault(enumerable.OfType<object>(), predict);

		public static object First(this IEnumerable enumerable) {
			if (enumerable is IList list) return list.Count > 0 ? list[0] : LinqEnumerable.First(Array.Empty<object>());
			var enumerator = enumerable.GetEnumerator();
			if (!enumerator.MoveNext()) return LinqEnumerable.First(Array.Empty<object>());; // throws exception
			var value = enumerator.Current;
			return value;
		}

		public static object First(this IEnumerable enumerable, Func<object, bool> predict) =>
			System.Linq.Enumerable.First(enumerable.OfType<object>(), predict);

		public static T FirstOrDefault<T>(this IEnumerable enumerable) {
			if (enumerable is IList list) return list.Count > 0 ? (T)list[0] : default;
			var enumerator = enumerable.GetEnumerator();
			if (!enumerator.MoveNext()) return default;
			var value = (T)enumerator.Current;
			return value;
		}

		public static T FirstOrDefault<T>(this IEnumerable enumerable, Func<T, bool> predict) =>
			System.Linq.Enumerable.FirstOrDefault<T>(enumerable.OfType<T>(), predict);

		public static T First<T>(this IEnumerable enumerable) {
			if (enumerable is IList list) return list.Count > 0 ? (T)list[0] : LinqEnumerable.First(Array.Empty<T>());
			var enumerator = enumerable.GetEnumerator();
			if (!enumerator.MoveNext()) return LinqEnumerable.First(Array.Empty<T>()); // throws exception
			var value = (T)enumerator.Current;
			return value;
		}

		public static T First<T>(this IEnumerable enumerable, Func<T, bool> predict) =>
			System.Linq.Enumerable.First<T>(enumerable.OfType<T>(), predict);

		public static object LastOrDefault(this IEnumerable enumerable) {
			if (enumerable is IList list) return list.Count > 0 ? list[^1] : null;
			var enumerator = enumerable.GetEnumerator();
			object last = DependencyProperty.UnsetValue;
			while (enumerator.MoveNext()) last = enumerator.Current;
			return last != DependencyProperty.UnsetValue ? last : null;
		}

		public static object LastOrDefault(this IEnumerable enumerable, Func<object, bool> predict) =>
			System.Linq.Enumerable.LastOrDefault(enumerable.OfType<object>(), predict);

		public static object LastOrDefault<T>(this IEnumerable enumerable) {
			if (enumerable is IList list) return list.Count > 0 ? (T)list[^1] : default;
			var enumerator = enumerable.GetEnumerator();
			object last = DependencyProperty.UnsetValue;
			while (enumerator.MoveNext()) last = enumerator.Current;
			return last != DependencyProperty.UnsetValue ? last : default;
		}

		public static T LastOrDefault<T>(this IEnumerable enumerable, Func<T, bool> predict) =>
			System.Linq.Enumerable.LastOrDefault<T>(enumerable.OfType<T>(), predict);

		public static object Last(this IEnumerable enumerable) {
			if (enumerable is IList list) return list.Count > 0 ? list[^1] : LinqEnumerable.Last(Array.Empty<object>());
			var enumerator = enumerable.GetEnumerator();
			object last = DependencyProperty.UnsetValue;
			while (enumerator.MoveNext()) last = enumerator.Current;
			return last != DependencyProperty.UnsetValue ? last : LinqEnumerable.Last(Array.Empty<object>());
		}

		public static object Last(this IEnumerable enumerable, Func<object, bool> predict) =>
			System.Linq.Enumerable.Last(enumerable.OfType<object>(), predict);

		public static object Last<T>(this IEnumerable enumerable) {
			if (enumerable is IList list) return list.Count > 0 ? (T)list[^1] : LinqEnumerable.Last(Array.Empty<T>());
			var enumerator = enumerable.GetEnumerator();
			object last = DependencyProperty.UnsetValue;
			while (enumerator.MoveNext()) last = enumerator.Current;
			return last != DependencyProperty.UnsetValue ? (T)last : LinqEnumerable.Last(Array.Empty<T>());
		}

		public static T Last<T>(this IEnumerable enumerable, Func<T, bool> predict) =>
			System.Linq.Enumerable.Last<T>(enumerable.OfType<T>(), predict);

		public static int Count(this IEnumerable source) {
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (source is System.Collections.Generic.ICollection<object> collectionoft) return collectionoft.Count;
			// if (source is IIListProvider<object> listProv) return listProv.GetCount(onlyIfCheap: false);
			if (source is ICollection collection) return collection.Count;

			int count = 0;
			var e = source.GetEnumerator();
			checked { while (e.MoveNext()) count++; }
			return count;
		}

	}

}
