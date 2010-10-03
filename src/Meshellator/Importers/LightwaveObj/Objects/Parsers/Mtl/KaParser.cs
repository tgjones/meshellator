namespace Meshellator.Importers.LightwaveObj.Objects.Parsers.Mtl
{
	public class KaParser : LineParser
	{
		private Vertex _ka;

		public override void Parse()
		{
			_ka = new Vertex();

			_ka.X = float.Parse(Words[1]);
			_ka.Y = float.Parse(Words[2]);
			_ka.Z = float.Parse(Words[3]);
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			Material currentMaterial = wavefrontObject.CurrentMaterial;
			currentMaterial.Ka = _ka;
		}
	}
}