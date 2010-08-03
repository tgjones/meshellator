using System.Runtime.InteropServices;
using Nexus;
using SlimDX.Direct3D9;

namespace Satis.ModelViewer.Framework.Rendering
{
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPositionNormalTexture
	{
		public Point3D Position;
		public Vector3D Normal;
		public Point2D TextureCoordinate;

		public VertexPositionNormalTexture(Point3D position, Vector3D normal, Point2D textureCoordinate)
		{
			Position = position;
			Normal = normal;
			TextureCoordinate = textureCoordinate;
		}

		public static VertexElement[] VertexElements
		{
			get
			{
				return new[] {
					new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
					new VertexElement(0, Vector3D.SizeInBytes, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
					new VertexElement(0, (short) (Vector3D.SizeInBytes * 2), DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
					VertexElement.VertexDeclarationEnd
				};
			}
		}

		public static int SizeInBytes
		{
			get { return Point3D.SizeInBytes + Vector3D.SizeInBytes + Point2D.SizeInBytes; }
		}
	}
}