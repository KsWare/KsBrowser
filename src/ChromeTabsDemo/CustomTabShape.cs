using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Demo {

	public class CustomTabShape : Shape {

		private double _lastRenderedWidth;

		public CustomTabShape() {
			Stretch = Stretch.Fill;
		}

		protected override Size MeasureOverride(Size constraint) {
			if (double.IsPositiveInfinity(constraint.Width) || double.IsPositiveInfinity(constraint.Height)) {
				return Size.Empty;
			}

			// we will size ourselves to fit the available space
			return constraint;
		}

		protected override Geometry DefiningGeometry => GetGeometry();

		private Geometry GetGeometry() {
			double width = DesiredSize.Width - StrokeThickness;
			double height = 25;
			double x1 = width - 15;
			double x2 = width - 10;
			double x3 = width - 2;
			double x4 = width - 0;
			double x5 = width - 0;
			//For some reason this is needed to update the visual if the initial width is 0
			if (ActualWidth > 0 && _lastRenderedWidth == 0)
				InvalidateVisual();
			_lastRenderedWidth = DesiredSize.Width;

			return Geometry.Parse(string.Format(CultureInfo.InvariantCulture,
				"M0,{5} C0,0 2,0 10,0 15,0 {0},0 {1},0 {2},0 {3},0 {4},{5}", x1, x2, x3, x4, x5, height));
		}
	}

}
