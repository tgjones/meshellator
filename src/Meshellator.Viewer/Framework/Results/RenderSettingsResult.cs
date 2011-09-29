using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Services;
using Meshellator.Viewer.Framework.Services;

namespace Meshellator.Viewer.Framework.Results
{
	public class RenderSettingsResult : IResult
	{
		public event EventHandler<ResultCompletionEventArgs> Completed;

		private readonly Action<IModelEditor> _setRenderSetting;

		[Import(typeof(IShell))]
		private IShell _shell;

		public RenderSettingsResult(Action<IModelEditor> setRenderSetting)
		{
			_setRenderSetting = setRenderSetting;
		}

		public void Execute(ActionExecutionContext context)
		{
			IModelEditor vm = (IModelEditor)_shell.ActiveItem;
			_setRenderSetting(vm);

			if (Completed != null)
				Completed(this, null);
		}
	}
}