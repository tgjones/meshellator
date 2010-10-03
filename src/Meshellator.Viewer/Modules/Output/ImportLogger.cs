using Gemini.Modules.Output;

namespace Meshellator.Viewer.Modules.Output
{
	public class ImportLogger : ILogger
	{
		private readonly IOutput _output;

		public ImportLogger(IOutput output)
		{
			_output = output;
		}

		public void Error(string message)
		{
			_output.Append("Error: " + message);
		}

		public void Warn(string message)
		{
			_output.Append("Warning: " + message);
		}
	}
}