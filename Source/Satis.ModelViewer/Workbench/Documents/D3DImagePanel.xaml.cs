using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using Satis.ModelViewer.Services.Direct3D;

namespace Satis.ModelViewer.Workbench.Documents
{
	/// <summary>
	/// Interaction logic for D3DImagePanel.xaml
	/// </summary>
	public partial class D3DImagePanel : UserControl
	{
		private IGraphicsDeviceService _graphicsDeviceService;
		private RenderWindow _renderWindow;
		private ArcBallCameraController _cameraController;

		public event EventHandler D3DImageRendering;

		#region RenderCommand property

		public static readonly DependencyProperty RenderCommandProperty = DependencyProperty.Register("RenderCommand", typeof(ICommand), typeof(D3DImagePanel));

		public ICommand RenderCommand
		{
			get { return (ICommand) GetValue(RenderCommandProperty); }
			set { SetValue(RenderCommandProperty, value); }
		}

		#endregion

		public int FramesPerSecond { get; set; }
		public IntPtr BackBufferPointer { get; set; }

		public D3DImagePanel()
		{
			InitializeComponent();
			Loaded += OnLoaded;
		}

		protected void OnLoaded(object sender, RoutedEventArgs e)
		{
			_graphicsDeviceService = new GraphicsDeviceService();
			_renderWindow = new RenderWindow((int) ActualWidth, (int) ActualHeight, _graphicsDeviceService.Device);
			BackBufferPointer = _renderWindow.GetBackBufferComPointer();

			_cameraController = new ArcBallCameraController(_renderWindow, this);
			_cameraController.CameraTransformed += OnCameraTransformed;

			/*DispatcherTimer timer = new DispatcherTimer
			{
				Interval = new TimeSpan(1000 / FramesPerSecond)
			};
			timer.Tick += OnTimerTick;
			timer.Start();*/

			Refresh();
		}

		void OnCameraTransformed(object sender, EventArgs e)
		{
			Refresh();
		}

		/*private void OnTimerTick(object sender, EventArgs e)
		{
			Refresh();
		}*/

		private void Refresh()
		{
			if (BackBufferPointer == IntPtr.Zero)
				return;

			D3DImageSource.Lock();

			// Calling SetBackBuffer repeatedly with the same IntPtr is a no-op, so there's no
			// performance penalty.
			D3DImageSource.SetBackBuffer(D3DResourceType.IDirect3DSurface9, BackBufferPointer);

			if (D3DImageSource.IsFrontBufferAvailable)
				OnD3DImageRendering(EventArgs.Empty);

			D3DImageSource.AddDirtyRect(new Int32Rect(0, 0, (int) ActualWidth, (int) ActualHeight));
			D3DImageSource.Unlock();
		}

		protected void OnD3DImageRendering(EventArgs args)
		{
			if (D3DImageRendering != null)
				D3DImageRendering(this, args);
			if (RenderCommand != null)
				RenderCommand.Execute(new RenderCommandParameters
				{
					GraphicsDevice = _graphicsDeviceService.Device,
					CameraTransform = _cameraController.Transform
				});
		}

		protected override Size MeasureOverride(Size constraint)
		{
			return new Size(D3DImageSource.PixelWidth, D3DImageSource.PixelHeight);
		}
	}
}
