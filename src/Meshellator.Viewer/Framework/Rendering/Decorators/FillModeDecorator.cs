using System.Collections.Generic;
using SharpDX.Direct3D9;

namespace Meshellator.Viewer.Framework.Rendering.Decorators
{
	internal class FillModeDecorator : DecoratorBase
	{
		private readonly Device _device;
		private SharpDX.Direct3D9.FillMode _fillMode;

		public FillModeDecorator(Device device)
		{
			_device = device;
		}

		public override void OnBeginDrawModel(Model model, RenderSettings renderSettings, IEnumerable<IDecorator> decorators)
		{
			_device.SetRenderState(RenderState.FillMode, _fillMode);
		}

		public override void OnEndDrawModel(Model model, RenderSettings renderSettings)
		{
			_device.SetRenderState(RenderState.FillMode, FillMode.Solid);
		}

		public override bool IsActive(RenderSettings settings)
		{
			switch (settings.Parameters.FillMode)
			{
				case FillMode.Solid:
					_fillMode = SharpDX.Direct3D9.FillMode.Solid;
					return true;
				case FillMode.Wireframe:
					_fillMode = SharpDX.Direct3D9.FillMode.Wireframe;
					return true;
				default:
					return false;
			}
		}
	}
}