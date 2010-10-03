using System;
using Nexus;

namespace Meshellator.Primitives
{
	public class SphereTessellator : PrimitiveTessellator
	{
		private readonly float _radius;

		public SphereTessellator(float radius, int tessellationLevel)
			: base(tessellationLevel)
		{
			if (tessellationLevel < 3)
				throw new ArgumentOutOfRangeException("tessellationLevel");

			_radius = radius;
		}

		public override void Tessellate()
		{
			int verticalSegments = TessellationLevel;
			int horizontalSegments = TessellationLevel * 2;

			// Start with a single vertex at the bottom of the sphere.
			AddVertex((Point3D)(Vector3D.Down * _radius), Vector3D.Down);

			// Create rings of vertices at progressively higher latitudes.
			for (int i = 0; i < verticalSegments - 1; i++)
			{
				float latitude = ((i + 1) * MathUtility.PI /
																		verticalSegments) - MathUtility.PI_OVER_2;

				float dy = MathUtility.Sin(latitude);
				float dxz = MathUtility.Cos(latitude);

				// Create a single ring of vertices at this latitude.
				for (int j = 0; j < horizontalSegments; j++)
				{
					float longitude = j * MathUtility.TWO_PI / horizontalSegments;

					float dx = (float)Math.Cos(longitude) * dxz;
					float dz = (float)Math.Sin(longitude) * dxz;

					Vector3D normal = new Vector3D(dx, dy, dz);

					AddVertex((Point3D)(normal * _radius), normal);
				}
			}

			// Finish with a single vertex at the top of the sphere.
			AddVertex((Point3D)(Vector3D.Up * _radius), Vector3D.Up);

			// Create a fan connecting the bottom vertex to the bottom latitude ring.
			for (int i = 0; i < horizontalSegments; i++)
			{
				AddIndex(0);
				AddIndex(1 + (i + 1) % horizontalSegments);
				AddIndex(1 + i);
			}

			// Fill the sphere body with triangles joining each pair of latitude rings.
			for (int i = 0; i < verticalSegments - 2; i++)
			{
				for (int j = 0; j < horizontalSegments; j++)
				{
					int nextI = i + 1;
					int nextJ = (j + 1) % horizontalSegments;

					AddIndex(1 + i * horizontalSegments + j);
					AddIndex(1 + i * horizontalSegments + nextJ);
					AddIndex(1 + nextI * horizontalSegments + j);

					AddIndex(1 + i * horizontalSegments + nextJ);
					AddIndex(1 + nextI * horizontalSegments + nextJ);
					AddIndex(1 + nextI * horizontalSegments + j);
				}
			}

			// Create a fan connecting the top vertex to the top latitude ring.
			for (int i = 0; i < horizontalSegments; i++)
			{
				AddIndex(CurrentVertex - 1);
				AddIndex(CurrentVertex - 2 - (i + 1) % horizontalSegments);
				AddIndex(CurrentVertex - 2 - i);
			}
		}
	}
}