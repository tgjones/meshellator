using System.Linq;
using Nexus;

namespace Meshellator.Importers.Nff.Parsers
{
	public class TessellationParser : LineParser
	{
		public override void Parse(ParserContext context, Scene scene, string[] words)
		{
			// "tess" level {primitive}

			int level = int.Parse(words[1]);
			context.CurrentTessellationLevel = level;

			string[] primitiveWords = words.Skip(2).ToArray();
			LineParser primitiveParser = LineParserFactory.GetParser(primitiveWords);
			primitiveParser.Parse(context, scene, primitiveWords);
		}
	}
}