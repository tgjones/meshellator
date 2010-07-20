using Nexus;
using Nexus.Graphics.Transforms;
using SlimDX;
using SlimDX.Direct3D9;

namespace Satis.ModelViewer.Services.Direct3D
{
	public class Renderer
	{
		private readonly Device _device;
		private readonly Model _model;
		private readonly Matrix _viewProjection;

		public Renderer(Device device, Model model, int width, int height, Transform3D cameraTransform)
		{
			_device = device;
			_model = model;

			Matrix3D projection = Matrix3D.CreatePerspectiveFieldOfView(
				MathUtility.PI_OVER_4,
				width / (float) height,
				1.0f, 6000.0f);
			Matrix3D view = Matrix3D.CreateLookAt(
				new Point3D(0, 800.0f, 1500.0f),
				Vector3D.Forward,
				Vector3D.Up);

			Matrix3D transform = Matrix3D.Invert(cameraTransform.Value) * view * projection;
			_viewProjection = new Matrix
			{
				M11 = transform.M11,
				M12 = transform.M12,
				M13 = transform.M13,
				M14 = transform.M14,

				M21 = transform.M21,
				M22 = transform.M22,
				M23 = transform.M23,
				M24 = transform.M24,

				M31 = transform.M31,
				M32 = transform.M32,
				M33 = transform.M33,
				M34 = transform.M34,

				M41 = transform.M41,
				M42 = transform.M42,
				M43 = transform.M43,
				M44 = transform.M44,
			};
		}

		public void Render()
		{
			_device.SetRenderState(RenderState.FillMode, FillMode.Solid);
			_device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new Color4(0.3f, 0.3f, 0.3f), 1.0f, 0);
			_device.BeginScene();

			_model.Draw(_viewProjection, Vector3D.Zero);
			_device.EndScene();
		}
	}
}