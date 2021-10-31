using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace KsWare.KsBrowser.Converter
{

	[MarkupExtensionReturnType(typeof(IValueConverter))]
	public class TimeLeftConverter : MarkupExtension, IValueConverter {
		/// <inheritdoc />
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}

		/// <inheritdoc />
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is DateTime dateTime && dateTime > DateTime.Now)
				value = dateTime.Subtract(DateTime.Now);

			if (value is TimeSpan timeSpan) {
				if (timeSpan.TotalSeconds < 90) return $"{timeSpan.TotalSeconds:F0}s";
				if (timeSpan.TotalMinutes < 90) return $"{timeSpan.TotalMinutes:F0}m";
				if (timeSpan.TotalHours < 36) return $"{timeSpan.TotalHours:F0}h";
				return $"{timeSpan.TotalDays:F0}d";
			}

			throw new ArgumentOutOfRangeException(nameof(value));
		}

		/// <inheritdoc />
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}

}
