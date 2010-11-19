using Meshellator.Primitives;
using Nexus;

namespace Meshellator.Importers.Nff.Parsers
{
	public class CylinderParser : PrimitiveParser
	{
		public override void Parse(ParserContext context, Scene scene, string[] words)
		{
			// "x-cylinder" center.x center.y center.z radius height
			Point3D center;
			center.X = float.Parse(words[1]);
			center.Y = float.Parse(words[2]);
			center.Z = float.Parse(words[3]);

			float radius = float.Parse(words[4]);
			float height = float.Parse(words[5]);

			CylinderTessellator tessellator = new CylinderTessellator(radius, height, context.CurrentTessellationLevel);

			Mesh mesh = CreateFromPrimitive(context, tessellator, center);
			scene.Meshes.Add(mesh);
		}
	}
}