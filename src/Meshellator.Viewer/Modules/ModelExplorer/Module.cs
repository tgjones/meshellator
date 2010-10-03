using System.Collections.Generic;
using System.Linq;
using Caliburn.Core;
using Caliburn.PresentationFramework;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Framework.Ribbon;
using Gemini.Framework.Services;
using Meshellator.Viewer.Framework.Scenes;
using Meshellator.Viewer.Modules.ModelEditor.ViewModels;
using Meshellator.Viewer.Modules.ModelExplorer.ViewModels;

namespace Meshellator.Viewer.Modules.ModelExplorer
{
	public class Module : ModuleBase
	{
		protected override IEnumerable<ComponentInfo> GetComponents()
		{
			yield return Singleton<IModelExplorer, ModelExplorerViewModel>();
		}

		protected override void Initialize()
		{
			// Hook into event which tells us when the active model changes.
			// When that happens we want to update the model explorer.
			Shell.ActiveDocumentChanged += OnShellActiveDocumentChanged;

			/*Ribbon.Tabs
				.First(x => x.Name == "Home")
				.Groups.First(x => x.Name == "Tools")
				.Add(new RibbonButton("Model Explorer", OpenModelExplorer));*/
		}

		private void OnShellActiveDocumentChanged(object sender, System.EventArgs e)
		{
			SceneViewModel scene = (Shell.CurrentPresenter is ModelEditorViewModel)
				? ((ModelEditorViewModel) Shell.CurrentPresenter).Scene
				: null;
			Container.GetInstance<IModelExplorer>().Scene = scene;
		}

		private static IEnumerable<IResult> OpenModelExplorer()
		{
			yield return Show.Tool<IModelExplorer>(Pane.Left);
		}
	}
}