// SOURCE TabPanel.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace KsWare.Presentation.Controls {

	/// <summary>
	/// TabPanel is a Panel designed to handle the intricacies of laying out the tab buttons in a TabControl.  Specically, it handles:
	/// Serving as an ItemsHost for TabItems within a TabControl 
	/// Determining correct sizing and positioning for TabItems 
	/// Handling the logic associated with MultiRow scenarios, namely: 
	/// Calculating row breaks in a collection of TabItems 
	/// Laying out TabItems in multiple rows based on those breaks 
	/// Performing specific layout for a selected item to indicate selection, namely: 
	/// Bringing the selected tab to the front, or, in other words, making the selected tab appear to be in front of other tabs. 
	/// Increasing the size pre-layout size of a selected item (note that this is not a transform, but rather an increase in the size allotted to the element in which to perform layout). 
	/// Bringing the selected tab to the front 
	/// Exposing attached properties that allow TabItems to be styled based on their placement within the TabPanel. 
	/// </summary>
	public class DocumentTabPanel : Panel {

		/// <summary>
		///     Default DependencyObject constructor
		/// </summary>
		/// <remarks>
		///     Automatic determination of current Dispatcher. Use alternative constructor
		///     that accepts a Dispatcher for best performance.
		/// </remarks>
		public DocumentTabPanel() : base() {
		}

		static DocumentTabPanel() {
			KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(DocumentTabPanel), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
			KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(DocumentTabPanel), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
		}

		/// <summary>
		/// Updates DesiredSize of the TabPanel.  Called by parent UIElement.  This is the first pass of layout.
		/// </summary>
		/// <remarks>
		/// TabPanel
		/// </remarks>
		/// <param name="constraint">Constraint size is an "upper limit" that TabPanel should not exceed.</param>
		/// <returns>TabPanel' desired size.</returns>
		protected override Size MeasureOverride(Size constraint) {
			var contentSize = new Size();
			var tabAlignment = TabStripPlacement;

			_numRows = 1;
			_numHeaders = 0;
			_rowHeight = 0;

			// For top and bottom placement the panel flow its children to calculate the number of rows and
			// desired vertical size
			if (tabAlignment == Dock.Top || tabAlignment == Dock.Bottom) {
				var visibleChildren = InternalChildren.OfType<UIElement>().Where(c => c.Visibility != Visibility.Collapsed).ToArray();
				_numHeaders = visibleChildren.Count();
				var maxWith = visibleChildren.Select(c => GetDesiredSizeWithoutMargin(c, constraint)).Sum();
				if (maxWith > constraint.Width) {
					contentSize.Width = constraint.Width;
					var average = constraint.Width / visibleChildren.Count();
					var smaller = visibleChildren.Where(c => GetDesiredSizeWithoutMargin(c).Width <= average).ToArray();
					average = (constraint.Width - smaller.Sum(c => GetDesiredSizeWithoutMargin(c).Width))/(visibleChildren.Count()-smaller.Count());
					constraint = new Size(average, constraint.Height);
					// visibleChildren.Except(smaller).ForEach(c=>c.Measure(constraint));
					foreach (var c in visibleChildren.Except(smaller)) c.Measure(constraint);
				}
				else {
					contentSize.Width = visibleChildren.Select(c => GetDesiredSizeWithoutMargin(c).Width).Sum();
				}
				_rowHeight = visibleChildren.Select(c => GetDesiredSizeWithoutMargin(c).Height).DefaultIfEmpty().Max();
				contentSize.Height = _rowHeight;
			}
			else if (tabAlignment == Dock.Left || tabAlignment == Dock.Right) {
				foreach (UIElement child in InternalChildren) {
					if (child.Visibility == Visibility.Collapsed)
						continue;

					_numHeaders++;

					// Helper measures child, and deals with Min, Max, and base Width & Height properties.
					// Helper returns the size a child needs to take up (DesiredSize or property specified size).
					child.Measure(constraint);

					var childSize = GetDesiredSizeWithoutMargin(child);

					if (contentSize.Width < childSize.Width)
						contentSize.Width = childSize.Width;

					contentSize.Height += childSize.Height;
				}
			}

			// Returns our minimum size & sets DesiredSize.
			return contentSize;
		}

		private double GetDesiredSizeWithoutMargin(UIElement element, Size constraint) {
			element.Measure(constraint); 
			return GetDesiredSizeWithoutMargin(element).Width;
		}

		/// <summary>
		/// TabPanel arranges each of its children.
		/// </summary>
		/// <param name="arrangeSize">Size that TabPanel will assume to position children.</param>
		protected override Size ArrangeOverride(Size arrangeSize) {
			var tabAlignment = TabStripPlacement;
			if (tabAlignment == Dock.Top || tabAlignment == Dock.Bottom) {
				ArrangeHorizontal(arrangeSize);
			}
			else if (tabAlignment == Dock.Left || tabAlignment == Dock.Right) {
				ArrangeVertical(arrangeSize);
			}

			return arrangeSize;
		}

		/// <summary>
		/// Override of <seealso cref="UIElement.GetLayoutClip"/>.
		/// </summary>
		/// <returns>Geometry to use as additional clip in case when element is larger then available space</returns>
		protected override Geometry GetLayoutClip(Size layoutSlotSize) {
			return null;
		}

		private Size GetDesiredSizeWithoutMargin(UIElement element) {
			var margin = (Thickness)element.GetValue(MarginProperty);
			var desiredSizeWithoutMargin = new Size();
			desiredSizeWithoutMargin.Height = Math.Max(0d, element.DesiredSize.Height - margin.Top - margin.Bottom);
			desiredSizeWithoutMargin.Width = Math.Max(0d, element.DesiredSize.Width - margin.Left - margin.Right);
			return desiredSizeWithoutMargin;
		}

		private double[] GetHeadersSize() {
			var headerSize = new double[_numHeaders];
			var childIndex = 0;
			foreach (UIElement child in InternalChildren) {
				if (child.Visibility == Visibility.Collapsed)
					continue;

				var childSize = GetDesiredSizeWithoutMargin(child);
				headerSize[childIndex] = childSize.Width;
				childIndex++;
			}

			return headerSize;
		}

		private void ArrangeHorizontal(Size arrangeSize) {
			var tabAlignment = TabStripPlacement;
			var isMultiRow = _numRows > 1;
			var activeRow = 0;
			var solution = Array.Empty<int>();
			var childOffset = new Vector();
			var headerSize = GetHeadersSize();

			// If we have multirows, then calculate the best header distribution
			if (isMultiRow) {
				solution = CalculateHeaderDistribution(arrangeSize.Width, headerSize);
				activeRow = GetActiveRow(solution);

				// TabPanel starts to layout children depend on activeRow which should be always on bottom (top)
				// The first row should start from Y = (_numRows - 1 - activeRow) * _rowHeight
				if (tabAlignment == Dock.Top)
					childOffset.Y = (_numRows - 1 - activeRow) * _rowHeight;

				if (tabAlignment == Dock.Bottom && activeRow != 0)
					childOffset.Y = (_numRows - activeRow) * _rowHeight;
			}

			var childIndex = 0;
			var separatorIndex = 0;
			foreach (UIElement child in InternalChildren) {
				if (child.Visibility == Visibility.Collapsed) continue;

				var margin = (Thickness)child.GetValue(MarginProperty);
				var leftOffset = margin.Left;
				var rightOffset = margin.Right;
				var topOffset = margin.Top;
				var bottomOffset = margin.Bottom;

				var lastHeaderInRow = isMultiRow &&
				                      (separatorIndex < solution.Length && solution[separatorIndex] == childIndex ||
				                       childIndex == _numHeaders - 1);

				//Length left, top, right, bottom;
				var cellSize = new Size(headerSize[childIndex], _rowHeight);

				// Align the last header in the row; If headers are not aligned directional nav would not work correctly
				if (lastHeaderInRow) {
					cellSize.Width = arrangeSize.Width - childOffset.X;
				}

				child.Arrange(new Rect(childOffset.X, childOffset.Y, cellSize.Width, cellSize.Height));

				var childSize = cellSize;
				childSize.Height = Math.Max(0d, childSize.Height - topOffset - bottomOffset);
				childSize.Width = Math.Max(0d, childSize.Width - leftOffset - rightOffset);

				// Calculate the offset for the next child
				childOffset.X += cellSize.Width;
				if (lastHeaderInRow) {
					if ((separatorIndex == activeRow && tabAlignment == Dock.Top) ||
					    (separatorIndex == activeRow - 1 && tabAlignment == Dock.Bottom))
						childOffset.Y = 0d;
					else
						childOffset.Y += _rowHeight;

					childOffset.X = 0d;
					separatorIndex++;
				}

				childIndex++;
			}
		}

		private void ArrangeVertical(Size arrangeSize) {
			var childOffsetY = 0d;
			foreach (UIElement child in InternalChildren) {
				if (child.Visibility != Visibility.Collapsed) {
					var childSize = GetDesiredSizeWithoutMargin(child);
					child.Arrange(new Rect(0, childOffsetY, arrangeSize.Width, childSize.Height));

					// Calculate the offset for the next child
					childOffsetY += childSize.Height;
				}
			}
		}

		// Returns the row which contain the child with IsSelected==true
		private int GetActiveRow(int[] solution) {
			var activeRow = 0;
			var childIndex = 0;
			if (solution.Length > 0) {
				foreach (UIElement child in InternalChildren) {
					if (child.Visibility == Visibility.Collapsed)
						continue;

					var isActiveTab = (bool)child.GetValue(Selector.IsSelectedProperty);

					if (isActiveTab) {
						return activeRow;
					}

					if (activeRow < solution.Length && solution[activeRow] == childIndex) {
						activeRow++;
					}

					childIndex++;
				}
			}

			// If the is no selected element and aligment is Top  - then the active row is the last row 
			if (TabStripPlacement == Dock.Top) {
				activeRow = _numRows - 1;
			}

			return activeRow;
		}

		/*   TabPanel layout calculation:
		 
		After measure call we have:
		rowWidthLimit - width of the TabPanel
		Header[0..n-1]  - headers
		headerWidth[0..n-1] - header width
		 
		Calculated values:
		numSeparators                       - number of separators between numSeparators+1 rows
		rowWidth[0..numSeparators]           - row width
		rowHeaderCount[0..numSeparators]    - Row Count = number of headers on that row
		rowAverageGap[0..numSeparators]     - Average Gap for the row i = (rowWidth - rowWidth[i])/rowHeaderCount[i]
		currentSolution[0..numSeparators-1] - separator currentSolution[i]=x means Header[x] and h[x+1] are separated with new line
		bestSolution[0..numSeparators-1]    - keep the last Best Solution
		bestSolutionRowAverageGap           - keep the last Best Solution Average Gap

		Between all separators distribution the best solution have minimum Average Gap - 
		this is the amount of pixels added to the header (to justify) in the row

		How does it work:
		First we flow the headers to calculate the number of necessary rows (numSeparators+1).
		That means we need to insert numSeparators separators between n headers (numSeparators<n always).
		For each current state rowAverageGap[1..numSeparators+1] are calculated for each row.
		Current state rowAverageGap = MAX (rowAverageGap[1..numSeparators+1]).
		Our goal is to find the solution with MIN (rowAverageGap).
		On each iteration step we move a header from a previous row to the row with maximum rowAverageGap.
		We countinue the itterations only if we move to better solution, i.e. rowAverageGap is smaller.
		Maximum iteration steps are less the number of headers.

		*/
		// Input: Row width and width of all headers
		// Output: int array which size is the number of separators and contains each separator position
		private int[] CalculateHeaderDistribution(double rowWidthLimit, double[] headerWidth) {
			double bestSolutionMaxRowAverageGap = 0;
			var numHeaders = headerWidth.Length;

			var numSeparators = _numRows - 1;
			double currentRowWidth = 0;
			var numberOfHeadersInCurrentRow = 0;
			double currentAverageGap = 0;
			var currentSolution = new int[numSeparators];
			var bestSolution = new int[numSeparators];
			var rowHeaderCount = new int[_numRows];
			var rowWidth = new double[_numRows];
			var rowAverageGap = new double[_numRows];
			var bestSolutionRowAverageGap = new double[_numRows];

			// Initialize the current state; Do the initial flow of the headers
			var currentRowIndex = 0;

			for (var index = 0; index < numHeaders; index++) {
				if (currentRowWidth + headerWidth[index] > rowWidthLimit && numberOfHeadersInCurrentRow > 0) {
					// if we cannot add next header - flow to next row
					// Store current row before we go to the next
					rowWidth[currentRowIndex] = currentRowWidth; // Store the current row width
					rowHeaderCount[currentRowIndex] =
						numberOfHeadersInCurrentRow; // For each row we store the number os headers inside
					currentAverageGap =
						Math.Max(0d,
							(rowWidthLimit - currentRowWidth) /
							numberOfHeadersInCurrentRow); // The amout of width that should be added to justify the header
					rowAverageGap[currentRowIndex] = currentAverageGap;
					currentSolution[currentRowIndex] = index - 1; // Separator points to the last header in the row
					if (bestSolutionMaxRowAverageGap <
					    currentAverageGap) // Remember the maximum of all currentAverageGap
						bestSolutionMaxRowAverageGap = currentAverageGap;

					// Iterate to next row
					currentRowIndex++;
					currentRowWidth = headerWidth[index]; // Accumulate header widths on the same row
					numberOfHeadersInCurrentRow = 1;
				}
				else {
					currentRowWidth += headerWidth[index]; // Accumulate header widths on the same row
					// Increase the number of headers only if they are not collapsed (width=0)
					if (headerWidth[index] != 0)
						numberOfHeadersInCurrentRow++;
				}
			}

			// If everithing fit in 1 row then exit (no separators needed)
			if (currentRowIndex == 0)
				return Array.Empty<int>();

			// Add the last row
			rowWidth[currentRowIndex] = currentRowWidth;
			rowHeaderCount[currentRowIndex] = numberOfHeadersInCurrentRow;
			currentAverageGap = (rowWidthLimit - currentRowWidth) / numberOfHeadersInCurrentRow;
			rowAverageGap[currentRowIndex] = currentAverageGap;
			if (bestSolutionMaxRowAverageGap < currentAverageGap)
				bestSolutionMaxRowAverageGap = currentAverageGap;

			currentSolution.CopyTo(bestSolution, 0); // Remember the first solution as initial bestSolution
			rowAverageGap.CopyTo(bestSolutionRowAverageGap,
				0); // bestSolutionRowAverageGap is used in ArrangeOverride to calculate header sizes

			// Search for the best solution
			// The exit condition if when we cannot move header to the next row 
			while (true) {
				// Find the row with maximum AverageGap
				var worstRowIndex = 0; // Keep the row index with maximum AverageGap
				double maxAG = 0;

				for (var i = 0; i < _numRows; i++) // for all rows
				{
					if (maxAG < rowAverageGap[i]) {
						maxAG = rowAverageGap[i];
						worstRowIndex = i;
					}
				}

				// If we are on the first row - cannot move from previous
				if (worstRowIndex == 0)
					break;

				// From the row with maximum AverageGap we try to move a header from previous row
				var moveToRow = worstRowIndex;
				var moveFromRow = moveToRow - 1;
				var moveHeader = currentSolution[moveFromRow];
				var movedHeaderWidth = headerWidth[moveHeader];

				rowWidth[moveToRow] += movedHeaderWidth;

				// If the moved header cannot fit - exit. We have the best solution already.
				if (rowWidth[moveToRow] > rowWidthLimit)
					break;

				// If header is moved successfully to the worst row
				// we update the arrays keeping the row state
				currentSolution[moveFromRow]--;
				rowHeaderCount[moveToRow]++;
				rowWidth[moveFromRow] -= movedHeaderWidth;
				rowHeaderCount[moveFromRow]--;
				rowAverageGap[moveFromRow] = (rowWidthLimit - rowWidth[moveFromRow]) / rowHeaderCount[moveFromRow];
				rowAverageGap[moveToRow] = (rowWidthLimit - rowWidth[moveToRow]) / rowHeaderCount[moveToRow];

				// EvaluateSolution:
				// If the current solution is better than bestSolution - keep it in bestSolution
				maxAG = 0;
				for (var i = 0; i < _numRows; i++) // for all rows
				{
					if (maxAG < rowAverageGap[i]) {
						maxAG = rowAverageGap[i];
					}
				}

				if (maxAG < bestSolutionMaxRowAverageGap) {
					bestSolutionMaxRowAverageGap = maxAG;
					currentSolution.CopyTo(bestSolution, 0);
					rowAverageGap.CopyTo(bestSolutionRowAverageGap, 0);
				}
			}

			// Each header size should be increased so headers in the row stretch to fit the row
			currentRowIndex = 0;
			for (var index = 0; index < numHeaders; index++) {
				headerWidth[index] += bestSolutionRowAverageGap[currentRowIndex];
				if (currentRowIndex < numSeparators && bestSolution[currentRowIndex] == index)
					currentRowIndex++;
			}

			// Use the best solution bestSolution[0..numSeparators-1] to layout
			return bestSolution;
		}

		private Dock TabStripPlacement {
			get {
				var placement = Dock.Top;
				var tc = TemplatedParent as TabControl;
				if (tc != null)
					placement = tc.TabStripPlacement;
				return placement;
			}
		}

		private int _numRows = 1; // Nubmer of row calculated in measure and used in arrange
		private int _numHeaders = 0; // Number of headers excluding the collapsed items
		private double _rowHeight = 0; // Maximum of all headers height
		private UIElement _newTabButton;

		// Decompiled with JetBrains decompiler
		// Type: MS.Internal.DoubleUtil
		// Assembly: WindowsBase, Version=5.0.10.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
		// MVID: 3AA0B4F5-B859-439C-B46E-3D299E3E3154
		// Assembly location: C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\5.0.10\WindowsBase.dll
		private class DoubleUtil {

			internal static bool IsNaN(double value) {
				var nanUnion = new NanUnion();
				nanUnion.DoubleValue = value;
				var num1 = nanUnion.UintValue & 18442240474082181120UL;
				var num2 = nanUnion.UintValue & 4503599627370495UL;
				return (num1 == 9218868437227405312UL || num1 == 18442240474082181120UL) && num2 > 0UL;
			}

			[StructLayout(LayoutKind.Explicit)]
			private struct NanUnion {
				[FieldOffset(0)] internal double DoubleValue;
				[FieldOffset(0)] internal ulong UintValue;
			}
		}

	}

}
