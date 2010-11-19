namespace Meshellator.Importers.Nff.Parsers
{
	public class ParserContext
	{
		public Material CurrentMaterial { get; set; }
		public int CurrentTessellationLevel { get; set; }

		public ParserContext()
		{
			CurrentMaterial = new Material();
			CurrentTessellationLevel = 20;
		}
	}
}