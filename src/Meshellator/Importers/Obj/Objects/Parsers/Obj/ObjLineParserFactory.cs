using Meshellator.Importers.Obj.Objects.Parsers.Mtl;

namespace Meshellator.Importers.Obj.Objects.Parsers.Obj
{
	public class ObjLineParserFactory : LineParserFactory
	{
		public ObjLineParserFactory(WavefrontObject @object)
			: base(@object)
		{
			Parsers.Add("v", new VertexParser());
			Parsers.Add("vn", new NormalParser());
			Parsers.Add("vp", new FreeFormParser());
			Parsers.Add("vt", new TextureCoordinateParser());
			Parsers.Add("f", new FaceParser(@object));
			Parsers.Add("#", new CommentParser());
			Parsers.Add("mtllib", new MaterialFileParser(@object));
			Parsers.Add("usemtl", new MaterialParser());
			Parsers.Add("g", new GroupParser());
		}
	}
}