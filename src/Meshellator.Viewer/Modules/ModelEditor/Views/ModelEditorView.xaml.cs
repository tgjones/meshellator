using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Meshellator.Viewer.Framework.Rendering;
using Meshellator.Viewer.Modules.ModelEditor.ViewModels;

namespace Meshellator.Viewer.Modules.ModelEditor.Views
{
	/// <summary>
	/// Interaction logic for ModelEditorView.xaml
	/// </summary>
	public partial class ModelEditorView : UserControl
	{
		private readonly Trackball _trackball = new Trackball();
		private IGraphicsDeviceService _graphicsDeviceService;
		private RenderWindow _renderWindow;
		private IntPtr _backBufferPointer;
		private Model _model;
		private bool _loaded;

		private Renderer _renderer;

		public ModelEditorView()
		{
			InitializeComponent();
			Loaded += OnLoaded;
		}

		protected void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (_loaded)
				return;

			_graphicsDeviceService = new GraphicsDeviceService();
			_renderWindow = new RenderWindow((int) ActualWidth, (int) ActualHeight, _graphicsDeviceService.Device);
			_backBufferPointer = _renderWindow.GetBackBufferComPointer();

			_trackball.TransformUpdated += OnTrackballTransformUpdated;
			_trackball.EventSource = this;

			_renderer = new Renderer(_graphicsDeviceService.Device, GetModel(),
				_graphicsDeviceService.Device.Viewport.Width, _graphicsDeviceService.Device.Viewport.Height,
				_trackball.Transform);

			ModelEditorViewModel vm = (ModelEditorViewModel) DataContext;
			vm.RenderParametersChanged += (sender2, e2) => Refresh();

			Refresh();

			_loaded = true;
		}

		private void OnTrackballTransformUpdated(object sender, EventArgs e)
		{
			Refresh();
		}

		private void Refresh()
		{
			if (_backBufferPointer == IntPtr.Zero)
				return;

			D3DImageSource.Lock();

			// Calling SetBackBuffer repeatedly with the same IntPtr is a no-op, so there's no
			// performance penalty.
			D3DImageSource.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _backBufferPointer);

			if (D3DImageSource.IsFrontBufferAvailable)
				Render();

			D3DImageSource.AddDirtyRect(new Int32Rect(0, 0, (int) ActualWidth, (int) ActualHeight));
			D3DImageSource.Unlock();
		}

		private Model GetModel()
		{
			if (_model == null)
			{
				ModelEditorViewModel vm = (ModelEditorViewModel) DataContext;
				_model = ModelConverter.FromScene(vm.Scene.Scene, _graphicsDeviceService.Device);
			}
			return _model;
		}

		private void Render()
		{
			ModelEditorViewModel vm = (ModelEditorViewModel) DataContext;
			_renderer.Render(vm.RenderParameters);
		}

		/*protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			if (_renderWindow != null)
			{
				_renderWindow.Width = (int) sizeInfo.NewSize.Width;
				_renderWindow.Height = (int) sizeInfo.NewSize.Height;
			}

			base.OnRenderSizeChanged(sizeInfo);
		}*/

		/*protected override Size MeasureOverride(Size constraint)
		{
			return new Size(D3DImageSource.PixelWidth, D3DImageSource.PixelHeight);
		}*/
	}
}
