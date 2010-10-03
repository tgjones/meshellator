namespace Meshellator.Importers.LightwaveObj.Objects
{
	public class Face
	{
		public static int GL_TRIANGLES = 1;
		public static int GL_QUADS = 2;

		public FaceType Type {get;set;}
		public int[] VertexIndices { get; set; }
		public int[] NormalIndices { get; set; }
		public int[] TextureIndices { get; set; }
		public Vertex[] Vertices {get;set;}
		public Vertex[] Normals {get;set;}
		public TextureCoordinate[] TextureCoordinates {get;set;}
	}
}