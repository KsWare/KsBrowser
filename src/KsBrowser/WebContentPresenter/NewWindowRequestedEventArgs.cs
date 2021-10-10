using System;

namespace KsWare.KsBrowser {

	public class NewWindowRequestedEventArgs : EventArgs {

		public NewWindowRequestedEventArgs(object internalArguments) {
			InternalArguments = internalArguments;
		}

		public object InternalArguments { get; }
	}

}