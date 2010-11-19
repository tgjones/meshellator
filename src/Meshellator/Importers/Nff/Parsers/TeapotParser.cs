using Meshellator.Primitives;
using Nexus;

namespace Meshellator.Importers.Nff.Parsers
{
	public class TeapotParser : PrimitiveParser
	{
		public override void Parse(ParserContext context, Scene scene, string[] words)
		{
			// "x-teapot" center.x center.y center.z size
			Point3D center;
			center.X = float.Parse(words[1]);
			center.Y = float.Parse(words[2]);
			center.Z = float.Parse(words[3]);

			float size = float.Parse(words[4]);

			TeapotTessellator tessellator = new TeapotTessellator(size, context.CurrentTessellationLevel);

			Mesh mesh = CreateFromPrimitive(context, tessellator, center);
			scene.Meshes.Add(mesh);
		}
	}
}