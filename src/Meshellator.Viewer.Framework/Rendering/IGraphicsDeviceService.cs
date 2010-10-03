using SlimDX.Direct3D9;

namespace Meshellator.Viewer.Framework.Rendering
{
	public interface IGraphicsDeviceService
	{
		Device Device { get; }
	}
}