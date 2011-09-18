namespace Meshellator.Importers.Obj.Objects.Parsers.Mtl
{
	public class MaterialParser : LineParser
	{
		private string _fileName;
		private string _materialName;

		public override void Parse(string fileName)
		{
			_fileName = fileName;
			base.Parse(fileName);
		}

		public override void Parse()
		{
			_materialName = Words[1];
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			Material newMaterial = new Material(_materialName, _fileName);

			wavefrontObject.Materials.Add(_materialName, newMaterial);
			wavefrontObject.CurrentMaterial = newMaterial;
		}
	}
}