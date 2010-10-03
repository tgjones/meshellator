using System;
using Nexus;

namespace Meshellator.Primitives
{
	public class TorusTessellator : PrimitiveTessellator
	{
		private readonly float _radius;
		private readonly float _thickness;

		protected override Vector3D PositionOffset
		{
			get { return new Vector3D(0, _thickness / 2, 0); }
		}

		public TorusTessellator(float radius, float thickness, int tessellationLevel)
			: base(tessellationLevel)
		{
			if (tessellationLevel < 3)
				throw new ArgumentOutOfRangeException("tessellationLevel");

			_radius = radius;
			_thickness = thickness;
		}

		public override void Tessellate()
		{
			// First we loop around the main ring of the torus.
			for (int i = 0; i < TessellationLevel; i++)
			{
				float outerAngle = i * MathUtility.TWO_PI / TessellationLevel;

				// Create a transform matrix that will align geometry to
				// slice perpendicularly though the current ring position.
				Matrix3D transform = Matrix3D.CreateTranslation(_radius, 0, 0) *
													 Matrix3D.CreateRotationY(outerAngle);

				// Now we loop along the other axis, around the side of the tube.
				for (int j = 0; j < TessellationLevel; j++)
				{
					float innerAngle = j * MathUtility.TWO_PI / TessellationLevel;

					float dx = (float)Math.Cos(innerAngle);
					float dy = (float)Math.Sin(innerAngle);

					// Create a vertex.
					Vector3D normal = new Vector3D(dx, dy, 0);
					Point3D position = (Point3D)(normal * _thickness / 2);

					position = Point3D.Transform(position, transform);
					normal = Vector3D.TransformNormal(normal, transform);

					AddVertex(position, normal);

					// And create indices for two triangles.
					int nextI = (i + 1) % TessellationLevel;
					int nextJ = (j + 1) % TessellationLevel;

					AddIndex(i * TessellationLevel + j);
					AddIndex(i * TessellationLevel + nextJ);
					AddIndex(nextI * TessellationLevel + j);

					AddIndex(i * TessellationLevel + nextJ);
					AddIndex(nextI * TessellationLevel + nextJ);
					AddIndex(nextI * TessellationLevel + j);
				}
			}
		}
	}
}