using System.Runtime.InteropServices;
using Nexus;
using SharpDX.Direct3D9;

namespace Meshellator.Viewer.Framework.Rendering
{
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexTransformedPositionTexture
	{
		public Point4D Position;
		public Point2D TextureCoordinate;

		public VertexTransformedPositionTexture(Point4D position, Point2D textureCoordinate)
		{
			Position = position;
			TextureCoordinate = textureCoordinate;
		}

		public static VertexElement[] VertexElements
		{
			get
			{
				return new[] {
					new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
					new VertexElement(0, (short) (Point4D.SizeInBytes * 2), DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
					VertexElement.VertexDeclarationEnd
				};
			}
		}

		public static int SizeInBytes
		{
			get { return Point4D.SizeInBytes + Point2D.SizeInBytes; }
		}
	}
}