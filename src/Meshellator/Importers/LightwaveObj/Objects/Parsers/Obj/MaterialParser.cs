namespace Meshellator.Importers.LightwaveObj.Objects.Parsers.Obj
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
			if (!wavefrontObject.Materials.ContainsKey(_materialName))
				return;

			Material newMaterial = wavefrontObject.Materials[_materialName];
			wavefrontObject.CurrentGroup.Material = newMaterial;
		}
	}
}