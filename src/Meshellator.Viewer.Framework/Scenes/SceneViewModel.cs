namespace Meshellator.Viewer.Framework.Scenes
{
	public class SceneViewModel
	{
		public Scene Scene { get; private set; }

		public SceneViewModel(Scene scene)
		{
			Scene = scene;
		}
	}
}