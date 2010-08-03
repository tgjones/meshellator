using System;
using Gemini.Framework;
using Satis.ModelViewer.Framework.Rendering;
using Satis.ModelViewer.Framework.Services;

namespace Satis.ModelViewer.Modules.ModelEditor.ViewModels
{
	public class ModelEditorViewModel : Screen, IModelEditor
	{
		public event EventHandler RenderParametersChanged;

		protected void OnRenderParametersChanged(EventArgs e)
		{
			EventHandler handler = RenderParametersChanged;
			if (handler != null) handler(this, e);
		}

		private bool _wireframe;
		private Scene _scene;
		private readonly string _title;
		private RenderParameters _renderParameters;

		public ModelEditorViewModel(Scene scene, string title)
		{
			_title = title;
			Scene = scene;
		}

		public override string DisplayName
		{
			get { return _title; }
		}

		public bool Wireframe
		{
			get { return _wireframe; }
			set
			{
				_wireframe = value;
				NotifyOfPropertyChange(() => Wireframe);
			}
		}

		public Scene Scene
		{
			get { return _scene; }
			set
			{
				_scene = value;
				NotifyOfPropertyChange(() => Scene);
			}
		}

		public RenderParameters RenderParameters
		{
			get
			{
				if (_renderParameters == null)
				{
					_renderParameters = new RenderParameters();
					_renderParameters.PropertyChanged += (sender, e) => OnRenderParametersChanged(e);
				}
				return _renderParameters;
			}
		}
	}
}