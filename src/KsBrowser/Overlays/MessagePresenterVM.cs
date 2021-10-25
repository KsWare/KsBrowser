using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.KsBrowser.Overlays {

	public class MessagePresenterVM : BaseMessageOverlayVM {

		/// <param name="baseMessageOverlayVms"></param>
		/// <inheritdoc />
		public MessagePresenterVM() {
			if(IsInDesignMode){
				Title = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr";
				Message = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.";
				IsOpen = true;
				return;
			}
			RegisterChildren(() => this);
			Fields[nameof(IsOpen)].ValueChangedEvent.add = OnIsOpenChanged;
		}

		public MessagePresenterVM(ListVM<BaseMessageOverlayVM> overlays) : this() {
			overlays.Add(this);
		}

		public string Title { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }
		public string Message { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		// private IEventSource<EventHandler> Closed => LazyWeakEventStore.Value.Get<EventHandler>();

		/// <summary>Displays a message box that has a message, title bar caption, button, and icon; and that accepts a default message box result, complies with the specified options, and returns a result.</summary>
		/// <param name="messageBoxText">A <see cref="T:System.String" /> that specifies the text to display.</param>
		/// <param name="caption">A <see cref="T:System.String" /> that specifies the title bar caption to display.</param>
		/// <param name="button">A <see cref="T:System.Windows.MessageBoxButton" /> value that specifies which button or buttons to display.</param>
		/// <param name="icon">A <see cref="T:System.Windows.MessageBoxImage" /> value that specifies the icon to display.</param>
		/// <param name="defaultResult">A <see cref="T:System.Windows.MessageBoxResult" /> value that specifies the default result of the message box.</param>
		/// <param name="options">A <see cref="T:System.Windows.MessageBoxOptions" /> value object that specifies the options.</param>
		/// <returns>A <see cref="T:System.Windows.MessageBoxResult" /> value that specifies which message box button is clicked by the user.</returns>
		public Task<MessageBoxResult> Show(
			string messageBoxText,
			string caption = null,
			MessageBoxButton button = MessageBoxButton.OK,
			MessageBoxImage icon = MessageBoxImage.None,
			MessageBoxResult defaultResult = MessageBoxResult.None,
			MessageBoxOptions options = MessageBoxOptions.None) {

			Title=caption;
			Message=messageBoxText;

			MessageOverlayButton[] buttons;

			switch (button) {
				case MessageBoxButton.OK:
					if(defaultResult==MessageBoxResult.None) defaultResult=MessageBoxResult.OK;
					buttons = new MessageOverlayButton[] {
						new MessageOverlayButton { Title = "_OK", Result = MessageBoxResult.OK, IsDefault = true }
					};
					break;
				case MessageBoxButton.YesNo:
					if(defaultResult==MessageBoxResult.None) defaultResult=MessageBoxResult.Yes;
					buttons = new MessageOverlayButton[] {
						new MessageOverlayButton { Title = "_Yes", Result = MessageBoxResult.Yes},
						new MessageOverlayButton { Title = "_No", Result = MessageBoxResult.No},
					};
					break;
				case MessageBoxButton.OKCancel:
					if(defaultResult==MessageBoxResult.None) defaultResult=MessageBoxResult.OK;
					buttons = new MessageOverlayButton[] {
						new MessageOverlayButton { Title = "_OK", Result = MessageBoxResult.OK},
						new MessageOverlayButton { Title = "_Cancel", Result = MessageBoxResult.Cancel},
					};
					break;
				case MessageBoxButton.YesNoCancel:
					if(defaultResult==MessageBoxResult.None) defaultResult=MessageBoxResult.Yes;
					buttons = new MessageOverlayButton[] {
						new MessageOverlayButton { Title = "_Yes", Result = MessageBoxResult.Yes},
						new MessageOverlayButton { Title = "_No", Result = MessageBoxResult.No},
						new MessageOverlayButton { Title = "_Cancel", Result = MessageBoxResult.Cancel},
					};
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(button));
			}
			buttons.ForEach(b => { b.IsDefault = defaultResult == b.Result; });

			Buttons = buttons;
			return ShowCore();
		}

	}

}
