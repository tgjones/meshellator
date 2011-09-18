namespace Meshellator.Importers.Obj.Objects.Parsers.Mtl
{
	public class NsParser : LineParser
	{
		private float _ns;

		public override void Parse()
		{
			_ns = float.Parse(Words[1]);
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			Material currentMaterial = wavefrontObject.CurrentMaterial;
			currentMaterial.Shininess = _ns;
		}
	}
}