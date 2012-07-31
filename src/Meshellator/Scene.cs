using System.Collections.Generic;
using Nexus;
using Nexus.Objects3D;

namespace Meshellator
{
	public class Scene
	{
		private AxisAlignedBox3D? _bounds;

		public string FileName { get; set; }
		public List<Mesh> Meshes { get; private set; }
		public List<Material> Materials { get; private set; }

		public AxisAlignedBox3D Bounds
		{
			get
			{
				if (_bounds == null)
				{
					_bounds = AxisAlignedBox3D.Empty;
					foreach (Mesh mesh in Meshes)
						_bounds = AxisAlignedBox3D.Union(_bounds.Value, mesh.Bounds);
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