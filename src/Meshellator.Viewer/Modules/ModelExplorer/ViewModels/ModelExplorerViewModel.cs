using System.Linq;
using Caliburn.PresentationFramework;
using Gemini.Framework;
using Meshellator.Viewer.Framework.Scenes;

namespace Meshellator.Viewer.Modules.ModelExplorer.ViewModels
{
	public class ModelExplorerViewModel : Screen, IModelExplorer
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

		public ModelExplorerViewModel(SceneViewModel scene)
		{
			Scenes = new BindableCollection<SceneViewModel>();
			Scenes.Add(scene);
		}
	}
}