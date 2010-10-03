using System.Collections.Generic;
using System.IO;

namespace Meshellator.Importers.LightwaveObj.Objects.Parsers.Mtl
{
	public class MaterialFileParser : LineParser
	{
		private Dictionary<string, Material> _materials = new Dictionary<string, Material>();
		private WavefrontObject _object;
		private MtlLineParserFactory parserFactory;

		public MaterialFileParser(WavefrontObject @object)
		{
			_object = @object;
			parserFactory = new MtlLineParserFactory(@object);
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			// Material are directly added by the parser, no need to do anything here...
		}

		public override void Parse()
		{
			string filename = Words[1];

			string pathToMTL = Path.Combine(_object.Contextfolder, filename);

			StreamReader reader = new StreamReader(pathToMTL);
			string currentLine = null;
			while ((currentLine = reader.ReadLine()) != null)
			{
				LineParser parser = parserFactory.GetLineParser(currentLine);
				parser.Parse();
				parser.IncorporateResults(_object);
			}

			reader.Close();

		}
	}
}