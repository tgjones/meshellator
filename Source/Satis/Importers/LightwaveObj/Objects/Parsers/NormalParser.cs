namespace Satis.Importers.LightwaveObj.Objects.Parsers
{
	public class NormalParser : LineParser
	{
		private Vertex _vertex;

		public override void Parse()
		{
			_vertex = new Vertex();

			_vertex.X = float.Parse(Words[1]);
			_vertex.Y = float.Parse(Words[2]);
			_vertex.Z = float.Parse(Words[3]);
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			wavefrontObject.Normals.Add(_vertex);
		}
	}
}