using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace KsWare.KsBrowser.Controls {

	public class ColumnDefinitionWidthConverterBinding : Binding, IValueConverter {

		public GridLength TrueValue { get; }
		public GridLength FalseValue { get; }

		/// <inheritdoc />
		public ColumnDefinitionWidthConverterBinding(string path, GridLength trueValue, GridLength falseValue) : base(path) {
			TrueValue = trueValue;
			FalseValue = falseValue;
			Converter = this;
		}

		/// <inheritdoc />
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is bool b) {
				return b ? TrueValue : FalseValue;
			}
			if (value is Visibility v) {
				return v==Visibility.Visible  ? TrueValue : FalseValue;
			}
			throw new ArgumentOutOfRangeException(nameof(value));
		}

		/// <inheritdoc />
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}

}
