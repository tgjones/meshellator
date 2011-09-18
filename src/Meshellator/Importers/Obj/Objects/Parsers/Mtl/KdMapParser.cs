using System.IO;

namespace Meshellator.Importers.Obj.Objects.Parsers.Mtl
{
	public class KdMapParser : LineParser
	{
		private readonly WavefrontObject _object;
		private string _texName;
		private string _fullTextureFileName;

		public KdMapParser(WavefrontObject @object)
		{
			_object = @object;
		}

		public override void Parse()
		{
			string textureFileName = Words[Words.Length - 1];
			_texName = textureFileName;
			_fullTextureFileName = Path.Combine(_object.Contextfolder, textureFileName);
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			Material currentMaterial = wavefrontObject.CurrentMaterial;
			currentMaterial.TextureName = _fullTextureFileName;
		}
	}
}