namespace Satis.Importers.LightwaveObj.Objects.Parsers.Mtl
{
	public class MaterialParser : LineParser
	{
		private string _materialName;

		public override void Parse()
		{
			_materialName = Words[1];
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			Material newMaterial = new Material(_materialName);

			wavefrontObject.Materials.Add(_materialName, newMaterial);
			wavefrontObject.CurrentMaterial = newMaterial;
		}
	}
}