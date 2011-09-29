using Nexus;
using SlimDX;

namespace Meshellator.Viewer.Framework.Rendering
{
	public static class ConversionExtensions
	{
		public static Matrix ToSlimDxMatrix(this Matrix3D matrix)
		{
			return new Matrix
			{
				M11 = matrix.M11,
				M12 = matrix.M12,
				M13 = matrix.M13,
				M14 = matrix.M14,
							
				M21 = matrix.M21,
				M22 = matrix.M22,
				M23 = matrix.M23,
				M24 = matrix.M24,
							
				M31 = matrix.M31,
				M32 = matrix.M32,
				M33 = matrix.M33,
				M34 = matrix.M34,
							
				M41 = matrix.M41,
				M42 = matrix.M42,
				M43 = matrix.M43,
				M44 = matrix.M44,
			};
		}

		public static Matrix3D ToMatrix3D(this Matrix matrix)
		{
			return new Matrix3D
			{
				M11 = matrix.M11,
				M12 = matrix.M12,
				M13 = matrix.M13,
				M14 = matrix.M14,

				M21 = matrix.M21,
				M22 = matrix.M22,
				M23 = matrix.M23,
				M24 = matrix.M24,

				M31 = matrix.M31,
				M32 = matrix.M32,
				M33 = matrix.M33,
				M34 = matrix.M34,

				M41 = matrix.M41,
				M42 = matrix.M42,
				M43 = matrix.M43,
				M44 = matrix.M44,
			};
		}

		public static Color4 ToColor4(this ColorF color)
		{
			return new Color4(color.A, color.R, color.G, color.B);
		}

		public static Color4 ToColor4(this Color color)
		{
			ColorF colorF = (ColorF) color;
			return new Color4(colorF.A, colorF.R, colorF.G, colorF.B);
		}
	}
}