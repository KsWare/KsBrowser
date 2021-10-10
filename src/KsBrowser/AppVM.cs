using KsWare.Presentation.ViewModelFramework;

namespace KsWare.KsBrowser {

	public class AppVM : ApplicationVM {

		/// <inheritdoc />
		public AppVM() {
			StartupUri = typeof(MainWindowVM);
		}
	}
}
