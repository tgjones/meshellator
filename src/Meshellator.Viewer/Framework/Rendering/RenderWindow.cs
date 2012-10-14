using System;
using System.ComponentModel;
using SharpDX.Direct3D9;

namespace Meshellator.Viewer.Framework.Rendering
{
	public class RenderWindow
	{
		private readonly Device _device;
		private readonly MultisampleType _multisampleType;

		private bool _surfaceSettingsChanged;
		private Surface _backBufferSurface;
		private Surface _depthStencilSurface;

		private int _width, _height;

		[DefaultValue(400), Category("Layout"), Description("Width of the scene window")]
		public int Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnSizeChanged();
			}
		}

		[DefaultValue(300), Category("Layout"), Description("Height of the scene window")]
		public int Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnSizeChanged();
			}
		}

		public IntPtr GetBackBufferComPointer()
		{
			if (_surfaceSettingsChanged)
			{
				if (_backBufferSurface != null)
					_backBufferSurface.Dispose();
				_backBufferSurface = Surface.CreateRenderTarget(_device, Width, Height,
					Format.X8R8G8B8, _multisampleType, 0, false);

				if (_depthStencilSurface != null)
					_depthStencilSurface.Dispose();

				_depthStencilSurface = Surface.CreateDepthStencil(_device, Width, Height,
					Format.D24S8, _multisampleType, 0, true);

				_surfaceSettingsChanged = false;
			}

			_device.SetRenderTarget(0, _backBufferSurface);
			_device.DepthStencilSurface = _depthStencilSurface;
			return _backBufferSurface.NativePointer;
		}

		public RenderWindow(int width, int height, Device device)
		{
			Width = width;
			Height = height;
			_device = device;
			_surfaceSettingsChanged = true;

			_multisampleType = new Direct3DEx().CheckDeviceMultisampleType(0, DeviceType.Hardware, Format.X8R8G8B8, true,
			                                                               MultisampleType.FourSamples)
			                   	? MultisampleType.FourSamples
			                   	: MultisampleType.None;
		}

		protected void OnSizeChanged()
		{
			_surfaceSettingsChanged = true;
		}
	}
}