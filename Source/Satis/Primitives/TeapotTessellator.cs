using System;
using Nexus;

namespace Satis.Primitives
{
	public class TeapotTessellator : BezierPrimitiveTessellator
	{
		private readonly float _size;

		protected override Vector3D PositionOffset
		{
			get { return new Vector3D(0, _size / 2, 0); }
		}

		public TeapotTessellator(float size, int tessellationLevel)
			: base(tessellationLevel)
		{
			if (tessellationLevel < 3)
				throw new ArgumentOutOfRangeException("tessellationLevel");

			_size = size;
		}

		protected override Point3D[] GetControlPoints()
		{
			return new[]
			{
				new Point3D(0f, 0.345f, -0.05f),
				new Point3D(-0.028f, 0.345f, -0.05f),
				new Point3D(-0.05f, 0.345f, -0.028f),
				new Point3D(-0.05f, 0.345f, -0f),
				new Point3D(0f, 0.3028125f, -0.334375f),
				new Point3D(-0.18725f, 0.3028125f, -0.334375f),
				new Point3D(-0.334375f, 0.3028125f, -0.18725f),
				new Point3D(-0.334375f, 0.3028125f, -0f),
				new Point3D(0f, 0.3028125f, -0.359375f),
				new Point3D(-0.20125f, 0.3028125f, -0.359375f),
				new Point3D(-0.359375f, 0.3028125f, -0.20125f),
				new Point3D(-0.359375f, 0.3028125f, -0f),
				new Point3D(0f, 0.27f, -0.375f),
				new Point3D(-0.21f, 0.27f, -0.375f),
				new Point3D(-0.375f, 0.27f, -0.21f),
				new Point3D(-0.375f, 0.27f, -0f),
				new Point3D(0f, 0.13875f, -0.4375f),
				new Point3D(-0.245f, 0.13875f, -0.4375f),
				new Point3D(-0.4375f, 0.13875f, -0.245f),
				new Point3D(-0.4375f, 0.13875f, -0f),
				new Point3D(0f, 0.007499993f, -0.5f),
				new Point3D(-0.28f, 0.007499993f, -0.5f),
				new Point3D(-0.5f, 0.007499993f, -0.28f),
				new Point3D(-0.5f, 0.007499993f, -0f),
				new Point3D(0f, -0.105f, -0.5f),
				new Point3D(-0.28f, -0.105f, -0.5f),
				new Point3D(-0.5f, -0.105f, -0.28f),
				new Point3D(-0.5f, -0.105f, -0f),
				new Point3D(0f, -0.105f, 0.5f),
				new Point3D(0f, -0.2175f, -0.5f),
				new Point3D(-0.28f, -0.2175f, -0.5f),
				new Point3D(-0.5f, -0.2175f, -0.28f),
				new Point3D(-0.5f, -0.2175f, -0f),
				new Point3D(0f, -0.27375f, -0.375f),
				new Point3D(-0.21f, -0.27375f, -0.375f),
				new Point3D(-0.375f, -0.27375f, -0.21f),
				new Point3D(-0.375f, -0.27375f, -0f),
				new Point3D(0f, -0.2925f, -0.375f),
				new Point3D(-0.21f, -0.2925f, -0.375f),
				new Point3D(-0.375f, -0.2925f, -0.21f),
				new Point3D(-0.375f, -0.2925f, -0f),
				new Point3D(0f, 0.17625f, 0.4f),
				new Point3D(-0.075f, 0.17625f, 0.4f),
				new Point3D(-0.075f, 0.2325f, 0.375f),
				new Point3D(0f, 0.2325f, 0.375f),
				new Point3D(0f, 0.17625f, 0.575f),
				new Point3D(-0.075f, 0.17625f, 0.575f),
				new Point3D(-0.075f, 0.2325f, 0.625f),
				new Point3D(0f, 0.2325f, 0.625f),
				new Point3D(0f, 0.17625f, 0.675f),
				new Point3D(-0.075f, 0.17625f, 0.675f),
				new Point3D(-0.075f, 0.2325f, 0.75f),
				new Point3D(0f, 0.2325f, 0.75f),
				new Point3D(0f, 0.12f, 0.675f),
				new Point3D(-0.075f, 0.12f, 0.675f),
				new Point3D(-0.075f, 0.12f, 0.75f),
				new Point3D(0f, 0.12f, 0.75f),
				new Point3D(0f, 0.06375f, 0.675f),
				new Point3D(-0.075f, 0.06375f, 0.675f),
				new Point3D(-0.075f, 0.007499993f, 0.75f),
				new Point3D(0f, 0.007499993f, 0.75f),
				new Point3D(0f, -0.04875001f, 0.625f),
				new Point3D(-0.075f, -0.04875001f, 0.625f),
				new Point3D(-0.075f, -0.09562501f, 0.6625f),
				new Point3D(0f, -0.09562501f, 0.6625f),
				new Point3D(-0.075f, -0.105f, 0.5f),
				new Point3D(-0.075f, -0.18f, 0.475f),
				new Point3D(0f, -0.18f, 0.475f),
				new Point3D(0f, 0.02624997f, -0.425f),
				new Point3D(-0.165f, 0.02624997f, -0.425f),
				new Point3D(-0.165f, -0.18f, -0.425f),
				new Point3D(0f, -0.18f, -0.425f),
				new Point3D(0f, 0.02624997f, -0.65f),
				new Point3D(-0.165f, 0.02624997f, -0.65f),
				new Point3D(-0.165f, -0.12375f, -0.775f),
				new Point3D(0f, -0.12375f, -0.775f),
				new Point3D(0f, 0.195f, -0.575f),
				new Point3D(-0.0625f, 0.195f, -0.575f),
				new Point3D(-0.0625f, 0.17625f, -0.6f),
				new Point3D(0f, 0.17625f, -0.6f),
				new Point3D(0f, 0.27f, -0.675f),
				new Point3D(-0.0625f, 0.27f, -0.675f),
				new Point3D(-0.0625f, 0.27f, -0.825f),
				new Point3D(0f, 0.27f, -0.825f),
				new Point3D(0f, 0.28875f, -0.7f),
				new Point3D(-0.0625f, 0.28875f, -0.7f),
				new Point3D(-0.0625f, 0.2934375f, -0.88125f),
				new Point3D(0f, 0.2934375f, -0.88125f),
				new Point3D(0f, 0.28875f, -0.725f),
				new Point3D(-0.0375f, 0.28875f, -0.725f),
				new Point3D(-0.0375f, 0.298125f, -0.8625f),
				new Point3D(0f, 0.298125f, -0.8625f),
				new Point3D(0f, 0.27f, -0.7f),
				new Point3D(-0.0375f, 0.27f, -0.7f),
				new Point3D(-0.0375f, 0.27f, -0.8f),
				new Point3D(0f, 0.27f, -0.8f),
				new Point3D(0f, 0.4575f, -0f),
				new Point3D(0f, 0.4575f, -0.2f),
				new Point3D(-0.1125f, 0.4575f, -0.2f),
				new Point3D(-0.2f, 0.4575f, -0.1125f),
				new Point3D(-0.2f, 0.4575f, -0f),
				new Point3D(0f, 0.3825f, -0f),
				new Point3D(0f, 0.27f, -0.35f),
				new Point3D(-0.196f, 0.27f, -0.35f),
				new Point3D(-0.35f, 0.27f, -0.196f),
				new Point3D(-0.35f, 0.27f, -0f),
				new Point3D(0f, 0.3075f, -0.1f),
				new Point3D(-0.056f, 0.3075f, -0.1f),
				new Point3D(-0.1f, 0.3075f, -0.056f),
				new Point3D(-0.1f, 0.3075f, -0f),
				new Point3D(0f, 0.3075f, -0.325f),
				new Point3D(-0.182f, 0.3075f, -0.325f),
				new Point3D(-0.325f, 0.3075f, -0.182f),
				new Point3D(-0.325f, 0.3075f, -0f),
				new Point3D(0f, 0.27f, -0.325f),
				new Point3D(-0.182f, 0.27f, -0.325f),
				new Point3D(-0.325f, 0.27f, -0.182f),
				new Point3D(-0.325f, 0.27f, -0f),
				new Point3D(0f, -0.33f, -0f),
				new Point3D(-0.1995f, -0.33f, -0.35625f),
				new Point3D(0f, -0.31125f, -0.375f),
				new Point3D(0f, -0.33f, -0.35625f),
				new Point3D(-0.35625f, -0.33f, -0.1995f),
				new Point3D(-0.375f, -0.31125f, -0f),
				new Point3D(-0.35625f, -0.33f, -0f),
				new Point3D(-0.21f, -0.31125f, -0.375f),
				new Point3D(-0.375f, -0.31125f, -0.21f),
			};
		}

		/// <summary>
		/// The teapot model consists of 10 bezier patches. Each patch has 16 control
		/// points, plus a flag indicating whether it should be mirrored in the Z axis
		/// as well as in X (all of the teapot is symmetrical from left to right, but
		/// only some parts are symmetrical from front to back). The control points
		/// are stored as integer indices into the TeapotControlPoints array.
		/// </summary>
		protected override BezierPatch[] GetPatches()
		{
			return new[]
			{
				// Rim.
				new BezierPatch(true, new int[]
				{
					102, 103, 104, 105, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15
				}),

				// Body.
				new BezierPatch(true, new int[]
				{
					12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27
				}),

				new BezierPatch(true, new int[]
				{
					24, 25, 26, 27, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40
				}),

				// Lid.
				new BezierPatch(true, new int[]
				{
					96, 96, 96, 96, 97, 98, 99, 100, 101, 101, 101, 101, 0, 1, 2, 3
				}),

				new BezierPatch(true, new int[]
				{
					0, 1, 2, 3, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117
				}),

				// Handle.
				new BezierPatch(false, new int[]
				{
					41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56
				}),

				new BezierPatch(false, new int[]
				{
					53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 28, 65, 66, 67
				}),

				// Spout.
				new BezierPatch(false, new int[]
				{
					68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83
				}),

				new BezierPatch(false, new int[]
				{
					80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95
				}),

				// Bottom.
				new BezierPatch(true, new int[]
				{
					118, 118, 118, 118, 124, 122, 119, 121,
					123, 126, 125, 120, 40, 39, 38, 37
				}),
			};
		}

		public override void Tessellate()
		{
			foreach (BezierPatch patch in Patches)
			{
				// Because the teapot is symmetrical from left to right, we only store
				// data for one side, then tessellate each patch twice, mirroring in X.
				TessellatePatch(patch, new Vector3D(_size, _size, _size));
				TessellatePatch(patch, new Vector3D(-_size, _size, _size));

				if (patch.MirrorZ)
				{
					// Some parts of the teapot (the body, lid, and rim, but not the
					// handle or spout) are also symmetrical from front to back, so
					// we tessellate them four times, mirroring in Z as well as X.
					TessellatePatch(patch, new Vector3D(_size, _size, -_size));
					TessellatePatch(patch, new Vector3D(-_size, _size, -_size));
				}
			}
		}

		/// <summary>
		/// Tessellates the specified bezier patch.
		/// </summary>
		private void TessellatePatch(BezierPatch patch, Vector3D scale)
		{
			// Look up the 16 control points for this patch.
			Point3D[] controlPoints = new Point3D[16];

			for (int i = 0; i < 16; i++)
			{
				int index = patch.Indices[i];
				controlPoints[i] = ControlPoints[index] * scale;
			}

			// Is this patch being mirrored?
			bool isMirrored = Math.Sign(scale.X) != Math.Sign(scale.Z);

			// Create the index and vertex data.
			CreatePatchIndices(isMirrored);
			CreatePatchVertices(controlPoints, isMirrored);
		}
	}
}