using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Input;
using Gemini.Contracts;
using Gemini.Contracts.Gui.Input;
using Gemini.Contracts.Gui.Layout;
using Satis.ModelViewer.Services.Direct3D;
using SlimDX;
using SlimDX.Direct3D9;

namespace Satis.ModelViewer.Workbench.Documents
{
	[Export(ContractNames.ExtensionPoints.Workbench.Documents, typeof(IDocument))] // so the Workbench can save/restore
	[Export(SatisContractNames.CompositionPoints.Workbench.Documents.ModelDocument, typeof(ModelDocument))] // so the new View Menu item can refer to it
	[Document(Name = DocumentName)]
	public class ModelDocument : AbstractDocument
	{
		public const string DocumentName = "ModelDocument";
		private Scene _scene;
		private Model _model;

		public ModelDocument()
		{
			Name = DocumentName;
		}

		public Scene Scene
		{
			get { return _scene; }
			set
			{
				_scene = value;
				Title = Path.GetFileName(value.FileName);
			}
		}

		private ICommand _renderCommand;

		public ICommand RenderCommand
		{
			get
			{
				if (_renderCommand == null)
					_renderCommand = new RelayCommand(ExecuteRenderCommand);
				return _renderCommand;
			}
		}

		private void ExecuteRenderCommand(object state)
		{
			RenderCommandParameters parameters = (RenderCommandParameters) state;
			Device graphicsDevice = parameters.GraphicsDevice;

			if (_model == null)
				_model = Model.FromScene(_scene, graphicsDevice);

			Renderer renderer = new Renderer(graphicsDevice, _model,
				graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height,
				parameters.CameraTransform);
			renderer.Render();
		}
	}
}