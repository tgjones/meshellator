using System.IO;
using Caliburn.PresentationFramework.ApplicationModel;
using Gemini.Framework.Services;
using Satis.ModelViewer.Modules.ModelEditor.ViewModels;

namespace Satis.ModelViewer.Modules.ModelEditor
{
	public class EditorProvider : IEditorProvider
	{
		public bool Handles(string path)
		{
			// TODO - change
			return true;
		}

		public IExtendedPresenter Create(string path)
		{
			return new ModelEditorViewModel(Meshellator.ImportFromFile(path), Path.GetFileName(path));
		}
	}
}