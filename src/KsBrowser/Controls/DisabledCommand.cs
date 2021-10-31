using System;
using System.Windows.Input;
using System.Windows.Markup;

namespace KsWare.KsBrowser.Controls {

	[MarkupExtensionReturnType(typeof(ICommand))]
	public class DisabledCommand : MarkupExtension, ICommand {

		public static ICommand Instance { get; } = new DisabledCommand();

		/// <inheritdoc />
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return Instance;
		}

		/// <inheritdoc />
		public bool CanExecute(object parameter) => false;

		/// <inheritdoc />
		public void Execute(object parameter) => throw new InvalidOperationException();

		/// <inheritdoc />
		public event EventHandler CanExecuteChanged;
	}

}
