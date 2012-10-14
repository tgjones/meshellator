using System;
using SharpDX.Direct3D9;

namespace Meshellator.Viewer.Framework.Rendering
{
	public class GraphicsDeviceService : IGraphicsDeviceService
	{
		public Device Device { get; private set; }

		public GraphicsDeviceService()
		{
			PresentParameters pp = new PresentParameters
			{
				SwapEffect = SwapEffect.Discard,
				DeviceWindowHandle = IntPtr.Zero,// windows[0].WindowHandle,
				Windowed = true,
				BackBufferWidth = 1,
				BackBufferHeight = 1,
				BackBufferFormat = Format.Unknown
			};

			Device = new DeviceEx(
					new Direct3DEx(),
					0, DeviceType.Hardware,
					IntPtr.Zero, //windows[0].WindowHandle,
					CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve,
					pp);
		}
	}
}