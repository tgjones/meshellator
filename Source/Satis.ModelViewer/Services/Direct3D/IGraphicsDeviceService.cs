using SlimDX.Direct3D9;

namespace Satis.ModelViewer.Services.Direct3D
{
	public interface IGraphicsDeviceService
	{
		Device Device { get; }
	}
}