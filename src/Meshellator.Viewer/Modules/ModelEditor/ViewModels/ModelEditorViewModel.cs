using System;
using Caliburn.Micro;
using Gemini.Framework;
using Meshellator.Viewer.Framework.Rendering;
using Meshellator.Viewer.Framework.Scenes;
using Meshellator.Viewer.Framework.Services;

namespace Meshellator.Viewer.Modules.ModelEditor.ViewModels
{
	public class ModelEditorViewModel : Document, IModelEditor
	{
		public event EventHandler RenderParametersChanged;

		protected void OnRenderParametersChanged(EventArgs e)
		{
			EventHandler handler = RenderParametersChanged;
			if (handler != null) handler(this, e);
		}

		private bool _wireframe;
		private SceneViewModel _scene;
		private readonly string _title;
		private RenderParameters _renderParameters;

		public ModelEditorViewModel(SceneViewModel scene, string title)
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

		public SceneViewModel Scene
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