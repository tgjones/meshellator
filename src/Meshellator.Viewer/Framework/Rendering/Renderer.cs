using Caliburn.Micro;
using Gemini.Framework.Services;
using Nexus;
using Nexus.Graphics.Transforms;
using Nexus.Objects3D;
using SlimDX;
using SlimDX.Direct3D9;

namespace Meshellator.Viewer.Framework.Rendering
{
	public class Renderer
	{
		#region Fields

		private readonly Device _device;
		private readonly Model _model;
		private readonly Transform3D _cameraTransform;
		private readonly Matrix3D _view, _projection;

		#endregion

		public Renderer(Device device, Model model, int width, int height, Transform3D cameraTransform)
		{
			_device = device;
			_model = model;
			_cameraTransform = cameraTransform;

			const float fov = MathUtility.PI_OVER_4;

			AxisAlignedBox3D bounds = _model.SourceScene.Bounds;
			Vector3D max = bounds.Size;
			float radius = System.Math.Max(max.X, System.Math.Max(max.Y, max.Z));

			_projection = Matrix3D.CreatePerspectiveFieldOfView(
				fov,
				width / (float) height,
				1.0f, radius * 10);

			float dist = radius / MathUtility.Sin(fov / 2);

			_view = Matrix3D.CreateLookAt(
				bounds.Center + Vector3D.Backward * dist,
				Vector3D.Forward,
				Vector3D.Up);
		}

		public void Render(RenderParameters parameters)
		{
			RenderSettings renderSettings = new RenderSettings();
			renderSettings.ProjectionMatrix = _projection;
			renderSettings.ViewMatrix = Matrix3D.Invert(_cameraTransform.Value) * _view;
			renderSettings.Parameters = parameters;

			IoC.Get<IStatusBar>().Message = "Camera Location: " + renderSettings.ViewMatrix.Translation;

			_device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new Color4(0.3f, 0.3f, 0.3f), 1.0f, 0);
			_device.BeginScene();

			_model.Draw(renderSettings);

			_device.EndScene();
		}
	}
}