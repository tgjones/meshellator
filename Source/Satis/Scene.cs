using System.Collections.Generic;
using Nexus;

namespace Satis
{
	public class Scene
	{
		private AxisAlignedBoundingBox? _bounds;

		public string FileName { get; set; }
		public List<Mesh> Meshes { get; private set; }
		public List<Material> Materials { get; private set; }

		public AxisAlignedBoundingBox Bounds
		{
			get
			{
				if (_bounds == null)
				{
					_bounds = new AxisAlignedBoundingBox();
					foreach (Mesh mesh in Meshes)
						_bounds = AxisAlignedBoundingBox.Union(_bounds.Value, mesh.Bounds);
				}
				return _bounds.Value;
			}
		}

		public Scene()
		{
			Meshes = new List<Mesh>();
			Materials = new List<Material>();
		}
	}
}