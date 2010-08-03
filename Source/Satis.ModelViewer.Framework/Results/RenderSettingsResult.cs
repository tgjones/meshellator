using System;
using Caliburn.PresentationFramework;
using Gemini.Framework.Services;
using Microsoft.Practices.ServiceLocation;
using Satis.ModelViewer.Framework.Services;

namespace Satis.ModelViewer.Framework.Results
{
	public class RenderSettingsResult : IResult
	{
		private readonly Action<IModelEditor> _setRenderSetting;

		public RenderSettingsResult(Action<IModelEditor> setRenderSetting)
		{
			_setRenderSetting = setRenderSetting;
		}

		public void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode)
		{
			IModelEditor vm = (IModelEditor) ServiceLocator.Current.GetInstance<IShell>().CurrentPresenter;
			_setRenderSetting(vm);

			if (Completed != null)
				Completed(this, null);
		}

		public event Action<IResult, Exception> Completed;
	}
}