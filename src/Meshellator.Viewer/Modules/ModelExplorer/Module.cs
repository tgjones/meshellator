using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Framework.Services;
using Meshellator.Viewer.Framework.Scenes;
using Meshellator.Viewer.Modules.ModelEditor.ViewModels;

namespace Meshellator.Viewer.Modules.ModelExplorer
{
	//[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
		[Import(typeof(IModelExplorer))]
		private IModelExplorer _modelExplorer;

		public override void Initialize()
		{
			// Hook into event which tells us when the active model changes.
			// When that happens we want to update the model explorer.
			//Shell.ActiveDocumentChanged += OnShellActiveDocumentChanged;

			/*Ribbon.Tabs
				.First(x => x.Name == "Home")
				.Groups.First(x => x.Name == "Tools")
				.Add(new RibbonButton("Model Explorer", OpenModelExplorer));*/
		}

		private void OnShellActiveDocumentChanged(object sender, System.EventArgs e)
		{
			SceneViewModel scene = (Shell.ActiveItem is ModelEditorViewModel)
				? ((ModelEditorViewModel) Shell.ActiveItem).Scene
				: null;
			_modelExplorer.Scene = scene;
		}

		private static IEnumerable<IResult> OpenModelExplorer()
		{
			yield return Show.Tool<IModelExplorer>();
		}
	}
}