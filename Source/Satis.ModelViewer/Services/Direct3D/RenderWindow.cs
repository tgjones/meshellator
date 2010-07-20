using System;
using System.ComponentModel;
using SlimDX.Direct3D9;

namespace Satis.ModelViewer.Services.Direct3D
{
	public class RenderWindow
	{
		private readonly Device _device;

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
					Format.X8R8G8B8, MultisampleType.None, 0, false);

				if (_depthStencilSurface != null)
					_depthStencilSurface.Dispose();

				_depthStencilSurface = Surface.CreateDepthStencil(_device, Width, Height,
					Format.D24S8, MultisampleType.None, 0, true);

				_surfaceSettingsChanged = false;
			}

			_device.SetRenderTarget(0, _backBufferSurface);
			_device.DepthStencilSurface = _depthStencilSurface;
			return _backBufferSurface.ComPointer;
		}

		public RenderWindow(int width, int height, Device device)
		{
			Width = width;
			Height = height;
			_device = device;
			_surfaceSettingsChanged = true;
		}

		protected void OnSizeChanged()
		{
			_surfaceSettingsChanged = true;
		}
	}
}