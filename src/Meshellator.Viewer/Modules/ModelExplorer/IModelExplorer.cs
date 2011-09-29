using Caliburn.Micro;
using Meshellator.Viewer.Framework.Scenes;

namespace Meshellator.Viewer.Modules.ModelExplorer
{
	public interface IModelExplorer : IScreen
	{
		SceneViewModel Scene { get; set; }
	}
}