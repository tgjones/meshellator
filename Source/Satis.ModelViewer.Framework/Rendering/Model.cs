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
			_device.SetRenderState(RenderState.ZEnable, true);
			_device.SetRenderState(RenderState.ZWriteEnable, true);

			// Draw opaque objects first.
			foreach (ModelMesh modelMesh in Meshes.Where(m => m.Opaque))
				modelMesh.Draw(_vertexDeclaration, settings, decorators);

			_device.SetRenderState(RenderState.ZWriteEnable, false);

			// Draw transparent objects. TODO: Sort by distance from camera and draw from back to front.
			_device.SetRenderState(RenderState.AlphaBlendEnable, true);
			_device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			_device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
			foreach (ModelMesh modelMesh in Meshes.Where(m => !m.Opaque))
				modelMesh.Draw(_vertexDeclaration, settings, decorators);
			_device.SetRenderState(RenderState.AlphaBlendEnable, false);

			_device.SetRenderState(RenderState.ZWriteEnable, true);
		}

		#endregion
	}
}