using Nexus;

namespace Meshellator
{
	public class Material
	{
		public string Name { get; set; }
		public string FileName { get; set; }

		public ColorRgbF AmbientColor { get; set; }
		public ColorRgbF DiffuseColor { get; set; }
		public ColorRgbF SpecularColor { get; set; }

		public string DiffuseTextureName { get; set; }
		public string SpecularTextureName { get; set; }

		// Phong-specific
		public int Shininess { get; set; }

		public float Transparency { get; set; }

		public Material()
		{
			AmbientColor = new ColorRgbF(0.1f, 0.1f, 0.1f);
			DiffuseColor = ColorsRgbF.White;
			SpecularColor = ColorsRgbF.White;
			Shininess = 16;
			Transparency = 1;
		}
	}
}