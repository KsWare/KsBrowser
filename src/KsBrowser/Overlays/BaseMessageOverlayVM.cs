using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.KsBrowser.Overlays {

	public class BaseMessageOverlayVM : ObjectVM{

		private SemaphoreSlim _signal;
		private MessageBoxResult _messageBoxResult = MessageBoxResult.None;

		/// <inheritdoc />
		public BaseMessageOverlayVM() {
			ButtonClickAction = new ActionVM { MːDoActionP = DoButton1Click, Parent = this};
			RegisterChildren(() => this);
		}

		public bool IsOpen { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }
		public ActionVM ButtonClickAction { get; [UsedImplicitly] private set; }
		public MessageOverlayButton[] Buttons { get => Fields.GetValue<MessageOverlayButton[]>(); protected set => Fields.SetValue(value); }

		protected void OnIsOpenChanged(object sender, ValueChangedEventArgs e) {
			if ((bool)e.NewValue) {
				_messageBoxResult = MessageBoxResult.None;
				_signal = new SemaphoreSlim(0, 1);
			}
			else {
				if(Parent is IListVM list) list.Remove(this);
				_signal.Release();
			}
		}

		/// <summary>
		/// Method for <see cref="ButtonClickAction"/>
		/// </summary>
		/// <param name="parameter">The command parameter.</param>
		private void DoButton1Click(object parameter) {
			switch (parameter) {
				case MessageBoxResult mbr: 
					_messageBoxResult = mbr; break;
				case string s: 
					if(!Enum.TryParse(s, true, out _messageBoxResult)) 
						throw new ArgumentOutOfRangeException(nameof(parameter)); break;
				default:
					throw new ArgumentOutOfRangeException(nameof(parameter));
			}
			IsOpen = false;
		}

		protected async Task<MessageBoxResult> ShowCore() {
			IsOpen = true;
			await _signal.WaitAsync();
			return _messageBoxResult;
		}
	}
}
