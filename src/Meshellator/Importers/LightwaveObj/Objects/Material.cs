namespace Meshellator.Importers.LightwaveObj.Objects
{
	public class Material
	{
		public Vertex Ka { get; set; }
		public Vertex Kd { get; set; }
		public Vertex Ks { get; set; }
		public float Shininess { get; set; }
		public string Name { get; set; }
		public string TextureName { get; set; }

		public Material(string name)
		{
			Name = name;
		}
	}
}