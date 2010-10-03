namespace Meshellator.Importers.LightwaveObj.Objects.Parsers
{
	public abstract class LineParser
	{
		public string[] Words { get; set; }

		public abstract void Parse();
		public abstract void IncorporateResults(WavefrontObject wavefrontObject);
	}
}