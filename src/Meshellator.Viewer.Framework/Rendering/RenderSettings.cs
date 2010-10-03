using Nexus;

namespace Meshellator.Viewer.Framework.Rendering
{
	internal class RenderSettings
	{
		public Matrix3D ViewMatrix { get; set; }
		public Matrix3D ProjectionMatrix { get; set; }

		public RenderParameters Parameters { get; set; }
	}
}