using Caliburn.Core;
using Nexus;

namespace Satis.ModelViewer.Framework.Rendering
{
	public class RenderParameters : PropertyChangedBase
	{
		private FillMode _fillMode;
		private bool _showNormals;
		private bool _showShadows;
		private bool _antiAliasingEnabled;
		private bool _noSpecular;
		private Vector3D _lightDirection = Vector3D.Normalize(new Vector3D(1, 1, 0));

		public FillMode FillMode
		{
			get { return _fillMode; }
			set
			{
				_fillMode = value;
				NotifyOfPropertyChange(() => FillMode);
			}
		}

		public bool ShowNormals
		{
			get { return _showNormals; }
			set
			{
				_showNormals = value;
				NotifyOfPropertyChange(() => ShowNormals);
			}
		}

		public bool ShowShadows
		{
			get { return _showShadows; }
			set
			{
				_showShadows = value;
				NotifyOfPropertyChange(() => ShowShadows);
			}
		}

		public bool AntiAliasingEnabled
		{
			get { return _antiAliasingEnabled; }
			set
			{
				_antiAliasingEnabled = value;
				NotifyOfPropertyChange(() => AntiAliasingEnabled);
			}
		}

		public bool NoSpecular
		{
			get { return _noSpecular; }
			set
			{
				_noSpecular = value;
				NotifyOfPropertyChange(() => NoSpecular);
			}
		}

		public Vector3D LightDirection
		{
			get { return _lightDirection; }
			set
			{
				_lightDirection = value;
				NotifyOfPropertyChange(() => LightDirection);
			}
		}
	}
}