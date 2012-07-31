using Nexus;
using Nexus.Graphics.Colors;

namespace Meshellator.Importers.Nff.Parsers
{
	public class MaterialParser : LineParser
	{
		public override void Parse(ParserContext context, Scene scene, string[] words)
		{
			// "f" red green blue Kd Ks Shine T index_of_refraction

			float r = float.Parse(words[1]);
			float g = float.Parse(words[2]);
			float b = float.Parse(words[3]);

			float kd = float.Parse(words[4]);
			float ks = float.Parse(words[5]);
			float shine = float.Parse(words[6]);

			ColorRgbF color = new ColorRgbF(r, g, b);

			Material material = new Material
			{
				DiffuseColor = color * kd,
				SpecularColor = color * ks,
				Shininess = (int)shine
			};

			scene.Materials.Add(material);
			context.CurrentMaterial = material;
		}
	}
}