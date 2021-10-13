// ORIGINAL: ChromeTabsDemo\ViewModel\TabClass4.cs

using KsWare.Presentation.ViewModels;

namespace Demo.ViewModel {

	public class Class4TabItemVM : ChromeTabItemVM {
		public string MyStringContent { get; set; }

		public bool IsBlinking {
			get => Fields.GetValue<bool>();
			set => Fields.SetValue(value);
		}
	}

}
