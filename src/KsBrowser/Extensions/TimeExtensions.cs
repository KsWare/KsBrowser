using System;

namespace KsWare.KsBrowser.Extensions {

	public static class TimeExtensions {

		public static TimeSpan FloorSeconds(this TimeSpan ts) {
			return TimeSpan.FromSeconds(Math.Floor(ts.TotalSeconds));
		}

		public static DateTime FloorSeconds(this DateTime dt) {
			return new DateTime((long)Math.Floor((decimal)dt.Ticks/10000000)*10000000);
		}

		public static TimeSpan CeilingSeconds(this TimeSpan ts) {
			return TimeSpan.FromSeconds(Math.Ceiling(ts.TotalSeconds));
		}

		public static TimeSpan RoundSeconds(this TimeSpan ts) {
			return TimeSpan.FromSeconds(Math.Round(ts.TotalSeconds));
		}
	}

}