namespace Meshellator.Importers.LightwaveObj.Objects.Parsers.Mtl
{
	public class KsParser : LineParser
	{
		private Vertex _ks;

		public override void Parse()
		{
			_ks = new Vertex();

			_ks.X = float.Parse(Words[1]);
			_ks.Y = float.Parse(Words[2]);
			_ks.Z = float.Parse(Words[3]);
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			Material currentMaterial = wavefrontObject.CurrentMaterial;
			currentMaterial.Ks = _ks;
		}
	}
}