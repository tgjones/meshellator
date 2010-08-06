using System;
using System.Collections.Generic;
using Nexus;
using Satis.ModelViewer.Framework.Rendering.Decorators;
using Satis.ModelViewer.Framework.Rendering.Effects;
using SlimDX.Direct3D9;

namespace Satis.ModelViewer.Framework.Rendering
{
	public class ModelMesh
	{
		#region Fields

		private readonly Device _device;
		private readonly VertexBuffer _vertexBuffer;
		private readonly int _numVertices;
		private readonly IndexBuffer _indexBuffer;
		private readonly int _primitiveCount;
		private readonly PrimitiveType _primitiveType;
		private readonly SimpleEffect _effect;

		#endregion

		#region Properties

		public SimpleEffect Effect
		{
			get { return _effect; }
		}

		public Mesh SourceMesh { get; private set; }

		public bool Opaque { get; private set; }

		#endregion

		#region Constructor

		internal ModelMesh(Mesh sourceMesh, Device device, VertexBuffer vertexBuffer, int numVertices,
			IndexBuffer indexBuffer, int primitiveCount,
			Matrix3D world, Material material, PrimitiveType primitiveType)
		{
			SourceMesh = sourceMesh;
			_device = device;
			_vertexBuffer = vertexBuffer;
			_numVertices = numVertices;
			_indexBuffer = indexBuffer;
			_primitiveCount = primitiveCount;
			_primitiveType = primitiveType;

			_effect = new SimpleEffect(device)
			{
				World = world,
				AmbientLightColor = new ColorRgbF(0.1f, 0.1f, 0.1f),
				DiffuseColor = material.DiffuseColor,
				SpecularColor = material.SpecularColor,
				SpecularPower = material.Shininess,
				Alpha = material.Transparency
			};
			_effect.CurrentTechnique = "RenderScene";
			Opaque = (material.Transparency == 1.0f);
		}

		#endregion

		#region Methods

		internal void Draw(VertexDeclaration vertexDeclaration, RenderSettings settings, IEnumerable<IDecorator> decorators)
		{
			_device.VertexDeclaration = vertexDeclaration;

			foreach (IDecorator decorator in decorators)
				decorator.OnBeginDrawMesh(this, settings);

			_device.SetStreamSource(0, _vertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
			_device.Indices = _indexBuffer;

			_effect.LightDirection = settings.Parameters.LightDirection;
			_effect.View = settings.ViewMatrix;
			_effect.Projection = settings.ProjectionMatrix;

			int passes = _effect.Begin();
			for (int i = 0; i < passes; i++)
			{
				_effect.BeginPass(i);
				
				_device.DrawIndexedPrimitives(
					_primitiveType, 0, 0, _numVertices,
					0, _primitiveCount);

				_effect.EndPass();
			}
			_effect.End();

			foreach (IDecorator decorator in decorators)
				decorator.OnEndDrawMesh(this, settings);
		}

		#endregion
	}
}