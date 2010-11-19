using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Meshellator.Importers.Nff.Parsers;
using Nexus;

namespace Meshellator.Importers.Nff
{
	[AssetImporter(".nff", "Neutral File Format")]
	public class NffImporter : AssetImporterBase
	{
		public override Scene ImportFile(FileStream fileStream, string fileName)
		{
			Scene scene = new Scene();

			ParserContext parserContext = new ParserContext();
			StreamReader reader = new StreamReader(fileName);
			string currentLine;
			while ((currentLine = reader.ReadLine()) != null)
				ParseLine(parserContext, scene, currentLine);
			reader.Close();

			return scene;
		}

		private static void ParseLine(ParserContext parserContext, Scene scene, string line)
		{
			string[] words = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			LineParser lineParser = LineParserFactory.GetParser(words);
			lineParser.Parse(parserContext, scene, words);
		}
	}
}