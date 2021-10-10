using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using static KsWare.Presentation.DocumentTabControl.AssemblyInfo;

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]

[assembly: XmlnsDefinition(XmlNamespace, RootNamespace)]
[assembly: XmlnsPrefix(XmlNamespace, Prefix)]

// namespace must equal to assembly name
// ReSharper disable once CheckNamespace
namespace KsWare.Presentation.DocumentTabControl {

	public static class AssemblyInfo {

		public static Assembly Assembly => Assembly.GetExecutingAssembly();

		public const string XmlNamespace = "http://ksware.de/Presentation/Controls";

		public const string RootNamespace = "KsWare.Presentation.Controls";

		public const string Prefix = "ksc";

	}
}