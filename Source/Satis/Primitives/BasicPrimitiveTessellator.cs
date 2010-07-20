using System;
using Nexus;

namespace Satis.Primitives
{
	public abstract class BasicPrimitiveTessellator
	{
		public Point3DCollection Positions { get; private set; }
		public Vector3DCollection Normals { get; private set; }
		public Int32Collection Indices { get; private set; }

		protected BasicPrimitiveTessellator()
		{
			Positions = new Point3DCollection();
			Normals = new Vector3DCollection();
			Indices = new Int32Collection();
		}

		protected int CurrentVertex
		{
			get { return Positions.Count; }
		}

		protected void AddVertex(Point3D position, Vector3D normal)
		{
			Positions.Add(position);
			Normals.Add(normal);
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