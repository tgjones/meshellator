using System;
using System.Diagnostics;
using Nexus;

namespace Satis.Primitives
{
	public abstract class BezierPrimitiveTessellator : PrimitiveTessellator
	{
		private Point3D[] _controlPoints;
		private BezierPatch[] _patches;

		public Point3D[] ControlPoints
		{
			get
			{
				if (_controlPoints == null)
					_controlPoints = GetControlPoints();
				return _controlPoints;
			}
		}

		public BezierPatch[] Patches
		{
			get
			{
				if (_patches == null)
					_patches = GetPatches();
				return _patches;
			}
		}

		protected abstract Point3D[] GetControlPoints();
		protected abstract BezierPatch[] GetPatches();

		protected BezierPrimitiveTessellator(int tessellationLevel)
			: base(tessellationLevel)
		{
		}

		/// <summary>
		/// Creates indices for a patch that is tessellated at the specified level.
		/// </summary>
		protected void CreatePatchIndices(bool isMirrored)
		{
			int stride = TessellationLevel + 1;

			for (int i = 0; i < TessellationLevel; i++)
			{
				for (int j = 0; j < TessellationLevel; j++)
				{
					// Make a list of six index values (two triangles).
					int[] indices =
						{
							i * stride + j,
							(i + 1) * stride + j,
							(i + 1) * stride + j + 1,

							i * stride + j,
							(i + 1) * stride + j + 1,
							i * stride + j + 1,
						};

					// If this patch is mirrored, reverse the
					// indices to keep the correct winding order.
					if (isMirrored)
						Array.Reverse(indices);

					// Create the indices.
					foreach (int index in indices)
						AddIndex(CurrentVertex + index);
				}
			}
		}


		/// <summary>
		/// Creates vertices for a patch that is tessellated at the specified level.
		/// </summary>
		protected void CreatePatchVertices(Point3D[] patch, bool isMirrored)
		{
			Debug.Assert(patch.Length == 16);

			for (int i = 0; i <= TessellationLevel; i++)
			{
				float ti = (float)i / TessellationLevel;

				for (int j = 0; j <= TessellationLevel; j++)
				{
					float tj = (float)j / TessellationLevel;

					// Perform four horizontal bezier interpolations
					// between the control points of this patch.
					Point3D p1 = Bezier(patch[0], patch[1], patch[2], patch[3], ti);
					Point3D p2 = Bezier(patch[4], patch[5], patch[6], patch[7], ti);
					Point3D p3 = Bezier(patch[8], patch[9], patch[10], patch[11], ti);
					Point3D p4 = Bezier(patch[12], patch[13], patch[14], patch[15], ti);

					// Perform a vertical interpolation between the results of the
					// previous horizontal interpolations, to compute the position.
					Point3D position = Bezier(p1, p2, p3, p4, tj);

					// Perform another four bezier interpolations between the control
					// points, but this time vertically rather than horizontally.
					Point3D q1 = Bezier(patch[0], patch[4], patch[8], patch[12], tj);
					Point3D q2 = Bezier(patch[1], patch[5], patch[9], patch[13], tj);
					Point3D q3 = Bezier(patch[2], patch[6], patch[10], patch[14], tj);
					Point3D q4 = Bezier(patch[3], patch[7], patch[11], patch[15], tj);

					// Compute vertical and horizontal tangent vectors.
					Vector3D tangentA = BezierTangent(p1, p2, p3, p4, tj);
					Vector3D tangentB = BezierTangent(q1, q2, q3, q4, ti);

					// Cross the two tangent vectors to compute the normal.
					Vector3D normal = Vector3D.Cross(tangentA, tangentB);

					if (normal.Length() > 0.0001f)
					{
						normal.Normalize();

						// If this patch is mirrored, we must invert the normal.
						if (isMirrored)
							normal = -normal;
					}
					else
					{
						// In a tidy and well constructed bezier patch, the preceding
						// normal computation will always work. But the classic teapot
						// model is not tidy or well constructed! At the top and bottom
						// of the teapot, it contains degenerate geometry where a patch
						// has several control points in the same place, which causes
						// the tangent computation to fail and produce a zero normal.
						// We 'fix' these cases by just hard-coding a normal that points
						// either straight up or straight down, depending on whether we
						// are on the top or bottom of the teapot. This is not a robust
						// solution for all possible degenerate bezier patches, but hey,
						// it's good enough to make the teapot work correctly!

						if (position.Y > 0)
							normal = Vector3D.Up;
						else
							normal = Vector3D.Down;
					}

					// Create the vertex.
					AddVertex(position, normal);
				}
			}
		}


		/// <summary>
		/// Performs a cubic bezier interpolation between four scalar control
		/// points, returning the value at the specified time (t ranges 0 to 1).
		/// </summary>
		private static float Bezier(float p1, float p2, float p3, float p4, float t)
		{
			return p1 * (1 - t) * (1 - t) * (1 - t) +
						 p2 * 3 * t * (1 - t) * (1 - t) +
						 p3 * 3 * t * t * (1 - t) +
						 p4 * t * t * t;
		}


		/// <summary>
		/// Performs a cubic bezier interpolation between four Point3D control
		/// points, returning the value at the specified time (t ranges 0 to 1).
		/// </summary>
		private static Point3D Bezier(Point3D p1, Point3D p2, Point3D p3, Point3D p4, float t)
		{
			Point3D result = new Point3D();

			result.X = Bezier(p1.X, p2.X, p3.X, p4.X, t);
			result.Y = Bezier(p1.Y, p2.Y, p3.Y, p4.Y, t);
			result.Z = Bezier(p1.Z, p2.Z, p3.Z, p4.Z, t);

			return result;
		}


		/// <summary>
		/// Computes the tangent of a cubic bezier curve at the specified time,
		/// when given four scalar control points.
		/// </summary>
		private static float BezierTangent(float p1, float p2, float p3, float p4, float t)
		{
			return p1 * (-1 + 2 * t - t * t) +
						 p2 * (1 - 4 * t + 3 * t * t) +
						 p3 * (2 * t - 3 * t * t) +
						 p4 * (t * t);
		}


		/// <summary>
		/// Computes the tangent of a cubic bezier curve at the specified time,
		/// when given four Point3D control points. This is used for calculating
		/// normals (by crossing the horizontal and vertical tangent vectors).
		/// </summary>
		private static Vector3D BezierTangent(Point3D p1, Point3D p2,
			Point3D p3, Point3D p4, float t)
		{
			Vector3D result = new Vector3D();

			result.X = BezierTangent(p1.X, p2.X, p3.X, p4.X, t);
			result.Y = BezierTangent(p1.Y, p2.Y, p3.Y, p4.Y, t);
			result.Z = BezierTangent(p1.Z, p2.Z, p3.Z, p4.Z, t);

			result.Normalize();

			return result;
		}
	}
}