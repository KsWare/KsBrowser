using System.Collections;

namespace KsWare.Presentation.Extensions {

	public static class EnumerableExtensions {

		public static object GetFirstOrDefault(this IEnumerable enumerable) {
			if (enumerable is IList list) return list.Count > 0 ? list[0] : null;
			var enumerator = enumerable.GetEnumerator();
			if (!enumerator.MoveNext()) return null;
			var value = enumerator.Current;
			return value;
		}

		public static T GetFirstOrDefaultEx<T>(this IEnumerable enumerable) {
			if (enumerable is IList list) return list.Count > 0 ? (T)list[0] : default;
			var enumerator = enumerable.GetEnumerator();
			if (!enumerator.MoveNext()) return default;
			var value = (T)enumerator.Current;
			return value;
		}

		public static int Count(this IEnumerable enumerable) {
			if (enumerable is IList list) return list.Count;
			var enumerator = enumerable.GetEnumerator();
			var count = 0;
			while (enumerator.MoveNext()) {
				count++;
			}

			return count;
		}

	}

}
