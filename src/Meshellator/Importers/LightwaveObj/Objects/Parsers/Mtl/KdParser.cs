namespace Meshellator.Importers.LightwaveObj.Objects.Parsers.Mtl
{
	public class KdParser : LineParser
	{
		private Vertex _kd;

		public override void Parse()
		{
			_kd = new Vertex();

			_kd.X = float.Parse(Words[1]);
			_kd.Y = float.Parse(Words[2]);
			_kd.Z = float.Parse(Words[3]);
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			Material currentMaterial = wavefrontObject.CurrentMaterial;
			currentMaterial.Kd = _kd;
		}
	}
}