using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Meshellator.Viewer.Framework.Scenes;

namespace Meshellator.Viewer.Modules.ModelExplorer.ViewModels
{
	[Export(typeof(IModelExplorer))]
	public class ModelExplorerViewModel : Tool, IModelExplorer
	{
		public IObservableCollection<SceneViewModel> Scenes { get; set; }

		public SceneViewModel Scene
		{
			get { return Scenes.FirstOrDefault(); }
			set
			{
				Scenes.Clear();
				Scenes.Add(value);
			}
		}

		public override PaneLocation PreferredLocation
		{
			get { return PaneLocation.Right; }
		}

		public ModelExplorerViewModel(SceneViewModel scene)
		{
			Scenes = new BindableCollection<SceneViewModel>();
			Scenes.Add(scene);
		}
	}
}