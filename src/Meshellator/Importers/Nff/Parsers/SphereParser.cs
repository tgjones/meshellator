using Meshellator.Primitives;
using Nexus;

namespace Meshellator.Importers.Nff.Parsers
{
	public class SphereParser : PrimitiveParser
	{
		public override void Parse(ParserContext context, Scene scene, string[] words)
		{
			// "s" center.x center.y center.z radius
			Point3D center;
			center.X = float.Parse(words[1]);
			center.Y = float.Parse(words[2]);
			center.Z = float.Parse(words[3]);

			float radius = float.Parse(words[4]);

			SphereTessellator tessellator = new SphereTessellator(radius, context.CurrentTessellationLevel);

			Mesh mesh = CreateFromPrimitive(context, tessellator, center);
			scene.Meshes.Add(mesh);
		}
	}
}