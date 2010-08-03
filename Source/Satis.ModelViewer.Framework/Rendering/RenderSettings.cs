using Nexus;

namespace Satis.ModelViewer.Framework.Rendering
{
	internal class RenderSettings
	{
		public Matrix3D ViewMatrix { get; set; }
		public Matrix3D ProjectionMatrix { get; set; }

		public RenderParameters Parameters { get; set; }
	}
}