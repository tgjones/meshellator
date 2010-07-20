using System.ComponentModel.Composition;
using System.Windows;
using Gemini.Contracts;

namespace Satis.ModelViewer.Workbench.Documents
{
	[Export(ContractNames.ExtensionPoints.Host.Views, typeof(ResourceDictionary))]
	public partial class ModelDocumentView : ResourceDictionary
	{
		public ModelDocumentView()
		{
			InitializeComponent();
		}
	}
}
