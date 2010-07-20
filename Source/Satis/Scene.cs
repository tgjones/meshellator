using System.Collections.Generic;

namespace Satis
{
	public class Scene
	{
		public string FileName { get; set; }
		public List<Mesh> Meshes { get; private set; }
		public List<Material> Materials { get; private set; }

		public Scene()
		{
			Meshes = new List<Mesh>();
			Materials = new List<Material>();
		}
	}
}