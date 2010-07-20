using Nexus.Graphics.Transforms;
using SlimDX.Direct3D9;

namespace Satis.ModelViewer.Services.Direct3D
{
	public class RenderCommandParameters
	{
		public Device GraphicsDevice { get; set; }
		public Transform3D CameraTransform { get; set; }
	}
}