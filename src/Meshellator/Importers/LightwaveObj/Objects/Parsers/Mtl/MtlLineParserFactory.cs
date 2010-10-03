namespace Meshellator.Importers.LightwaveObj.Objects.Parsers.Mtl
{
	public class MtlLineParserFactory : LineParserFactory
	{
		public MtlLineParserFactory(WavefrontObject @object)
			: base(@object)
		{
			Parsers.Add("newmtl", new MaterialParser());
			Parsers.Add("Ka", new KaParser());
			Parsers.Add("Kd", new KdParser());
			Parsers.Add("Ks", new KsParser());
			Parsers.Add("Ns", new NsParser());
			Parsers.Add("map_Kd", new KdMapParser(@object));
			Parsers.Add("#", new CommentParser());
		}
	}
}