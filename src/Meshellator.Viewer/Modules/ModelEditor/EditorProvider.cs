using System.ComponentModel.Composition;
using System.IO;
using Caliburn.Micro;
using Gemini.Framework.Services;
using Meshellator.Viewer.Framework.Scenes;
using Meshellator.Viewer.Modules.ModelEditor.ViewModels;

namespace Meshellator.Viewer.Modules.ModelEditor
{
	[Export(typeof(IEditorProvider))]
	public class EditorProvider : IEditorProvider
	{
		public bool Handles(string path)
		{
			return MeshellatorLoader.IsSupportedFormat(path);
		}

		public IScreen Create(string path)
		{
			return new ModelEditorViewModel(new SceneViewModel(MeshellatorLoader.ImportFromFile(path)), Path.GetFileName(path));
		}
	}
}