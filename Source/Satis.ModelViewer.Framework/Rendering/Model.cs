using System.Collections.Generic;
using System.Linq;
using Satis.ModelViewer.Framework.Rendering.Decorators;
using SlimDX.Direct3D9;

namespace Satis.ModelViewer.Framework.Rendering
{
	/// <summary>
	/// Summary description for Model.
	/// </summary>
	public class Model
	{
		#region Variables

		private readonly Device _device;
		private readonly VertexDeclaration _vertexDeclaration;
		private readonly List<IDecorator> _decorators;

		#endregion

		#region Properties

		public List<ModelMesh> Meshes { get; set; }

		public Scene SourceScene { get; private set; }

		#endregion

		#region Constructor

		internal Model(Scene scene, Device device, VertexDeclaration vertexDeclaration)
		{
			SourceScene = scene;

			_device = device;
			_vertexDeclaration = vertexDeclaration;

			_decorators = new List<IDecorator>
			{
				new FillModeDecorator(device),
				new NormalsDecorator(device),
				new ShadowDecorator(device),
				new RenderOptionsDecorator(device)
			};

			Meshes = new List<ModelMesh>();
		}

		#endregion

		#region Methods

		private IEnumerable<IDecorator> GetActiveDecorators(RenderSettings settings)
		{
			return _decorators.Where(d => d.IsActive(settings));
		}

		internal void Draw(RenderSettings settings)
		{
			IEnumerable<IDecorator> activeDecorators = GetActiveDecorators(settings);
			foreach (IDecorator decorator in activeDecorators)
				decorator.OnBeginDrawModel(this, settings, activeDecorators);

			DrawInternal(settings, activeDecorators);

			foreach (IDecorator decorator in activeDecorators)
				decorator.OnEndDrawModel(this, settings);
		}

		internal void DrawInternal(RenderSettings settings, IEnumerable<IDecorator> decorators)
		{
			foreach (ModelMesh modelMesh in Meshes)
				modelMesh.Draw(_vertexDeclaration, settings, decorators);
		}

		#endregion
	}
}