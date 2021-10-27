using System;

namespace KsWare.Presentation.ViewModels {

	public interface ITabItemCreationOptions {
		// ChromeTabItemVM Referrer { get; }
		ChromeTabItemVM GetInstance();
	}

	public class TabItemCreationOptions : ITabItemCreationOptions {

		public TabItemCreationOptions(bool activate = true, ChromeTabItemVM referrer = null) {
			Activate = activate;
			Referrer = referrer;
		}

		public TabItemCreationOptions(Type createInstance, object[] createInstanceArguments =null, bool activate = true, ChromeTabItemVM referrer = null) {
			CreateInstance = createInstance;
			CreateInstanceArguments = createInstanceArguments;
			Activate = activate;
			Referrer = referrer;
		}

		public TabItemCreationOptions(ChromeTabItemVM instance, bool activate = true, ChromeTabItemVM referrer = null) {
			Instance = instance;
			Activate = activate;
			Referrer = referrer;
		}

		public TabItemCreationOptions(Func<ChromeTabItemVM> createInstanceFunction, bool activate = true, ChromeTabItemVM referrer = null) {
			CreateInstanceFunction = createInstanceFunction;
			Activate = activate;
			Referrer = referrer;
		}

		public Type CreateInstance { get; }
		public object[] CreateInstanceArguments { get; }

		public ChromeTabItemVM Instance { get; }

		public Func<ChromeTabItemVM> CreateInstanceFunction { get; }

		public bool Activate { get; set; } = true;

		public int InsertPosition { get; set; } = -1;

		public ChromeTabItemVM Referrer { get; }

		ChromeTabItemVM ITabItemCreationOptions.GetInstance() {
			if (Instance != null) return Instance;
			if (CreateInstance != null) return (ChromeTabItemVM)Activator.CreateInstance(CreateInstance, CreateInstanceArguments);
			if (CreateInstanceFunction != null) return CreateInstanceFunction();
			return new ChromeTabItemVM();
		}

	}

}