using Meshellator.Primitives;
using Nexus;
using Nexus.Graphics.Transforms;

namespace Meshellator.Importers.Nff.Parsers
{
	public abstract class PrimitiveParser : LineParser
	{
		protected static Mesh CreateFromPrimitive(ParserContext context, BasicPrimitiveTessellator tessellator, Point3D center)
		{
			tessellator.Tessellate();

			Mesh mesh = new Mesh();
			mesh.Positions.AddRange(tessellator.Positions);
			mesh.Indices.AddRange(tessellator.Indices);
			mesh.Normals.AddRange(tessellator.Normals);
			mesh.Material = context.CurrentMaterial;

			mesh.Transform = new TranslateTransform
			{
				OffsetX = center.X,
				OffsetY = center.Y,
				OffsetZ = center.Z
			};

			return mesh;
		}
	}
}