using System;
using System.ComponentModel.Composition;
using Gemini.Contracts;
using Gemini.Contracts.Services.OutputService;

namespace Satis.ModelViewer.Services
{
	[Export(typeof(ILogger))]
	public class ImportLogger : ILogger
	{
		[Import(ContractNames.Services.Output.OutputService, typeof(IOutputService))]
		private Lazy<IOutputService> OutputService { get; set; }

		public void Error(string message)
		{
			OutputService.Value.Append("Error: " + message);
		}

		public void Warn(string message)
		{
			OutputService.Value.Append("Warning: " + message);
		}
	}
}