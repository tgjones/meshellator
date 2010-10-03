using Caliburn.PresentationFramework.ApplicationModel;
using Meshellator.Viewer.Framework.Scenes;

namespace Meshellator.Viewer.Modules.ModelExplorer
{
	public interface IModelExplorer : IExtendedPresenter
	{
		SceneViewModel Scene { get; set; }
	}
}