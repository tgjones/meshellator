using Nexus;

namespace Satis
{
	public class Material
	{
		public string Name { get; set; }

		public Color AmbientColor { get; set; }
		public Color DiffuseColor { get; set; }
		public Color SpecularColor { get; set; }

		public string DiffuseTextureName { get; set; }
		public string SpecularTextureName { get; set; }

		// Phong-specific
		public int Shininess { get; set; }

		public float Transparency { get; set; }
	}
}