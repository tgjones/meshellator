using System;
using Nexus;

namespace Satis
{
	public class Mesh
	{
		private AxisAlignedBoundingBox? _bounds;

		public Point3DCollection Positions { get; private set; }
		public Vector3DCollection Normals { get; private set; }
		public Point3DCollection TextureCoordinates { get; private set; }
		public Int32Collection Indices { get; private set; }

		public PrimitiveTopology PrimitiveTopology { get; set; }

		public int PrimitiveCount
		{
			get
			{
				switch (PrimitiveTopology)
				{
					case PrimitiveTopology.TriangleList :
						return Indices.Count / 3;
					case PrimitiveTopology.TriangleStrip :
						return Indices.Count - 2;
					default :
						throw new NotSupportedException();
				}
			}
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
			PrimitiveTopology = PrimitiveTopology.TriangleList;
		}
	}
}