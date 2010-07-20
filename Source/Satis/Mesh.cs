using System;
using Nexus;

namespace Satis
{
	public class Mesh
	{
		public Point3DCollection Positions { get; private set; }
		public Vector3DCollection Normals { get; private set; }
		public Point3DCollection TextureCoordinates { get; private set; }
		public Int32Collection Indices { get; private set; }

		public string MaterialName { get; set; }

		public Mesh()
		{
			Positions = new Point3DCollection();
			Normals = new Vector3DCollection();
			TextureCoordinates = new Point3DCollection();
			Indices = new Int32Collection();
		}
	}
}