using System;
using Nexus;

namespace Meshellator.Primitives
{
	public class CylinderTessellator : PrimitiveTessellator
	{
		private readonly float _radius;
		private readonly float _height;

		public CylinderTessellator(float radius, float height, int tessellationLevel)
			: base(tessellationLevel)
		{
			if (tessellationLevel < 3)
				throw new ArgumentOutOfRangeException("tessellationLevel");

			_radius = radius;
			_height = height / 2;
		}

		public override void Tessellate()
		{
			// Create a ring of triangles around the outside of the cylinder.
			for (int i = 0; i < TessellationLevel; i++)
			{
				Vector3D normal = GetCircleVector(i);

				AddVertex((Point3D)(normal * _radius + Vector3D.Up * _height), normal);
				AddVertex((Point3D)(normal * _radius + Vector3D.Down * _height), normal);

				AddIndex(i * 2);
				AddIndex(i * 2 + 1);
				AddIndex((i * 2 + 2) % (TessellationLevel * 2));

				AddIndex(i * 2 + 1);
				AddIndex((i * 2 + 3) % (TessellationLevel * 2));
				AddIndex((i * 2 + 2) % (TessellationLevel * 2));
			}

			// Create flat triangle fan caps to seal the top and bottom.
			CreateCap(Vector3D.Up);
			CreateCap(Vector3D.Down);
		}

		/// <summary>
		/// Helper method creates a triangle fan to close the ends of the cylinder.
		/// </summary>
		private void CreateCap(Vector3D normal)
		{
			// Create cap indices.
			for (int i = 0; i < TessellationLevel - 2; i++)
			{
				if (normal.Y > 0)
				{
					AddIndex(CurrentVertex);
					AddIndex(CurrentVertex + (i + 1) % TessellationLevel);
					AddIndex(CurrentVertex + (i + 2) % TessellationLevel);
				}
				else
				{
					AddIndex(CurrentVertex);
					AddIndex(CurrentVertex + (i + 2) % TessellationLevel);
					AddIndex(CurrentVertex + (i + 1) % TessellationLevel);
				}
			}

			// Create cap vertices.
			for (int i = 0; i < TessellationLevel; i++)
			{
				Point3D position = (Point3D)GetCircleVector(i) * _radius + normal * _height;
				AddVertex(position, normal);
			}
		}


		/// <summary>
		/// Helper method computes a point on a circle.
		/// </summary>
		private Vector3D GetCircleVector(int i)
		{
			float angle = i * MathUtility.TWO_PI / TessellationLevel;

			float dx = MathUtility.Cos(angle);
			float dz = MathUtility.Sin(angle);

			return new Vector3D(dx, 0, dz);
		}
	}
}