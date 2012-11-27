namespace Meshellator.Importers.Autodesk3ds
{
	public class Texture
	{
		public string MapName { get; set; }

		public float TextureBlend { get; set; }

		public float OffsetU { get; set; }
		public float OffsetV { get; set; }
		public float ScaleU { get; set; }
		public float ScaleV { get; set; }
		public float Rotation { get; set; }

		public TextureMapMode MapMode { get; set; }

		public Texture()
		{
			OffsetU = 0.0f;
			OffsetV = 0.0f;
			ScaleU = 1.0f;
			ScaleV = 1.0f;
			Rotation = 0.0f;
		}
	}
}