namespace Meshellator.Importers.Nff.Parsers
{
	public abstract class LineParser
	{
		public abstract void Parse(ParserContext context, Scene scene, string[] words);
	}
}