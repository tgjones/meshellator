using System;
using Nexus;

namespace Meshellator.Primitives
{
	public abstract class BasicPrimitiveTessellator
	{
		public Point3DCollection Positions { get; private set; }
		public Vector3DCollection Normals { get; private set; }
		public Int32Collection Indices { get; private set; }
		public Point2DCollection TextureCoordinates { get; private set; }

		public virtual PrimitiveTopology PrimitiveTopology
		{
			get { return PrimitiveTopology.TriangleList; }
		}

		protected BasicPrimitiveTessellator()
		{
			Positions = new Point3DCollection();
			Normals = new Vector3DCollection();
			Indices = new Int32Collection();
			TextureCoordinates = new Point2DCollection();
		}

		protected virtual Vector3D PositionOffset
		{
			get { return Vector3D.Zero; }
		}

		protected int CurrentVertex
		{
			get { return Positions.Count; }
		}

		protected void AddVertex(Point3D position, Vector3D normal)
		{
			Positions.Add(position + PositionOffset);
			Normals.Add(normal);
		}

		protected void AddVertex(Point3D position, Vector3D normal, Point2D textureCoordinate)
		{
			AddVertex(position, normal);
			TextureCoordinates.Add(textureCoordinate);
		}

		protected void AddIndex(int index)
		{
			if (index > int.MaxValue)
				throw new ArgumentOutOfRangeException("index");

			Indices.Add(index);
		}

		public abstract void Tessellate();
	}
}