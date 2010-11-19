using Meshellator.Primitives;
using Nexus;

namespace Meshellator.Importers.Nff.Parsers
{
	public class TorusParser : PrimitiveParser
	{
		public override void Parse(ParserContext context, Scene scene, string[] words)
		{
			// "x-plane" center.x center.y center.z width height
			Point3D center;
			center.X = float.Parse(words[1]);
			center.Y = float.Parse(words[2]);
			center.Z = float.Parse(words[3]);

			float radius = float.Parse(words[4]);
			float thickness = float.Parse(words[5]);

			TorusTessellator tessellator = new TorusTessellator(radius, thickness, context.CurrentTessellationLevel);

			Mesh mesh = CreateFromPrimitive(context, tessellator, center);
			scene.Meshes.Add(mesh);
		}
	}
}