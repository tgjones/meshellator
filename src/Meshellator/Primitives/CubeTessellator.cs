using Nexus;

namespace Meshellator.Primitives
{
	public class CubeTessellator : BasicPrimitiveTessellator
	{
		private readonly float _size;

		public CubeTessellator(float size)
		{
			_size = size;
		}

		public override void Tessellate()
		{
			// A cube has six faces, each one pointing in a different direction.
			Vector3D[] normals =
				{
					new Vector3D(0, 0, 1),
					new Vector3D(0, 0, -1),
					new Vector3D(1, 0, 0),
					new Vector3D(-1, 0, 0),
					new Vector3D(0, 1, 0),
					new Vector3D(0, -1, 0),
				};

			// Create each face in turn.
			foreach (Vector3D normal in normals)
			{
				// Get two vectors perpendicular to the face normal and to each other.
				Vector3D side1 = new Vector3D(normal.Y, normal.Z, normal.X);
				Vector3D side2 = Vector3D.Cross(normal, side1);

				// Six indices (two triangles) per face.
				AddIndex(CurrentVertex + 0);
				AddIndex(CurrentVertex + 1);
				AddIndex(CurrentVertex + 2);

				AddIndex(CurrentVertex + 0);
				AddIndex(CurrentVertex + 2);
				AddIndex(CurrentVertex + 3);

				// Four vertices per face.
				AddVertex((Point3D)((normal - side1 - side2) * _size / 2), normal);
				AddVertex((Point3D)((normal - side1 + side2) * _size / 2), normal);
				AddVertex((Point3D)((normal + side1 + side2) * _size / 2), normal);
				AddVertex((Point3D)((normal + side1 - side2) * _size / 2), normal);
			}
		}
	}
}