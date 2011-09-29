using System.Collections.Generic;
using SlimDX.Direct3D9;

namespace Meshellator.Viewer.Framework.Rendering.Decorators
{
	internal class RenderOptionsDecorator : DecoratorBase
	{
		private readonly Device _device;
		private bool _currentAntiAliasing;

		public RenderOptionsDecorator(Device device)
		{
			_device = device;
		}

		public override void OnBeginDrawModel(Model model, RenderSettings renderSettings, IEnumerable<IDecorator> decorators)
		{
			if (renderSettings.Parameters.AntiAliasingEnabled != _currentAntiAliasing)
			{
				_currentAntiAliasing = renderSettings.Parameters.AntiAliasingEnabled;
				_device.SetRenderState(RenderState.MultisampleAntialias, _currentAntiAliasing);
			}
		}

		public override void OnBeginDrawMesh(ModelMesh mesh, RenderSettings renderSettings)
		{
			mesh.Effect.SpecularEnabled = !renderSettings.Parameters.NoSpecular;
		}

		public override bool IsActive(RenderSettings settings)
		{
			return true;
		}
	}
}