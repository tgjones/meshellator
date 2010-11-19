using Meshellator.Primitives;
using Nexus;

namespace Meshellator.Importers.Nff.Parsers
{
	public class PlaneParser : PrimitiveParser
	{
		public override void Parse(ParserContext context, Scene scene, string[] words)
		{
			// "x-plane" center.x center.y center.z width height
			Point3D center;
			center.X = float.Parse(words[1]);
			center.Y = float.Parse(words[2]);
			center.Z = float.Parse(words[3]);

			int width = int.Parse(words[4]);
			int height = int.Parse(words[5]);

			PlaneTessellator tessellator = new PlaneTessellator(width, height);

			Mesh mesh = CreateFromPrimitive(context, tessellator, center);
			scene.Meshes.Add(mesh);
		}
	}
}