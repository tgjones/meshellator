using System;
using Nexus;
using Nexus.Graphics.Transforms;

namespace Meshellator
{
	public class Mesh
	{
		private AxisAlignedBoundingBox? _bounds;

		public string Name { get; set; }
		public Point3DCollection Positions { get; private set; }
		public Vector3DCollection Normals { get; private set; }
		public Point3DCollection TextureCoordinates { get; private set; }
		public Int32Collection Indices { get; private set; }

		public Transform3D Transform { get; set; }

		public int PrimitiveCount
		{
			get { return Indices.Count / 3; }
		}

		public Material Material { get; set; }

		public AxisAlignedBoundingBox Bounds
		{
			get
			{
				if (_bounds == null)
					_bounds = new AxisAlignedBoundingBox(Positions);
				return _bounds.Value;
			}
		}

		public Mesh()
		{
			Positions = new Point3DCollection();
			Normals = new Vector3DCollection();
			TextureCoordinates = new Point3DCollection();
			Indices = new Int32Collection();
			Transform = new MatrixTransform(Matrix3D.Identity);
		}
	}
}