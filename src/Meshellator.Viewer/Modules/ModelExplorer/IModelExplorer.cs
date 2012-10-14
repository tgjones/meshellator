using Gemini.Framework;
using Meshellator.Viewer.Framework.Scenes;

namespace Meshellator.Viewer.Modules.ModelExplorer
{
	public interface IModelExplorer : ITool
	{
		SceneViewModel Scene { get; set; }
	}
}