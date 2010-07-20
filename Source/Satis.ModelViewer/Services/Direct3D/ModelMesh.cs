using System;
using Nexus;
using SlimDX;
using SlimDX.Direct3D9;

namespace Satis.ModelViewer.Services.Direct3D
{
	public class ModelMesh
	{
		private Device _device;
		private VertexBuffer _vertexBuffer;
		private int _numVertices;
		private IndexBuffer _indexBuffer;
		private readonly int _primitiveCount;
		private Effect _effect;
		private readonly Material _material;
		private EffectHandle _wvpHandle;
		private EffectHandle _wHandle;
		private EffectHandle _diffuseHandle;

		public AxisAlignedBoundingBox Bounds { get; set; }

		internal ModelMesh(Device device, VertexBuffer vertexBuffer, int numVertices,
			IndexBuffer indexBuffer, int primitiveCount, Effect effect,
			Matrix world, Material material, AxisAlignedBoundingBox bounds)
		{
			Bounds = bounds;
			_device = device;
			_vertexBuffer = vertexBuffer;
			_numVertices = numVertices;
			_indexBuffer = indexBuffer;
			_primitiveCount = primitiveCount;
			_effect = effect;
			_material = material;

			_wvpHandle = _effect.GetParameter(null, "WorldViewProjection");
			_wHandle = _effect.GetParameter(null, "World");
			_diffuseHandle = _effect.GetParameter(null, "Diffuse");
		}

		public void Draw(Matrix viewProjection, Vector3D eyePosition)
		{
			_device.SetStreamSource(0, _vertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
			_device.Indices = _indexBuffer;

			_effect.SetValue(_wHandle, Matrix.Identity);
			_effect.SetValue(_wvpHandle, viewProjection);
			_effect.SetValue(_effect.GetParameter(null, "DiffuseColor"), ToVector3D(_material.DiffuseColor));
			_effect.SetValue(_effect.GetParameter(null, "SpecularColor"), ToVector3D(_material.SpecularColor));
			_effect.SetValue(_effect.GetParameter(null, "SpecularPower"), (float) _material.Shininess);
			_effect.SetValue(_effect.GetParameter(null, "Alpha"), _material.Transparency);
			_effect.SetValue(_effect.GetParameter(null, "EyePosition"), eyePosition);

			int passes = _effect.Begin();
			for (int i = 0; i < passes; i++)
			{
				_effect.BeginPass(i);
				/*ApplyMaterial(basicEffect, m_pSubsets[i].Material);
				if (m_pSubsets[i].Texture != null)
				{
					basicEffect.Texture = m_pSubsets[i].Texture;
					basicEffect.TextureEnabled = true;
				}
				else
				{
					basicEffect.TextureEnabled = false;
				}*/
				_device.DrawIndexedPrimitives(
					PrimitiveType.TriangleList, 0, 0, _numVertices,
					0, _primitiveCount);
				_effect.EndPass();
			}
			_effect.End();
		}

		private static Vector3D ToVector3D(Color color)
		{
			ColorF colorF = (ColorF) color;
			return new Vector3D(colorF.R, colorF.G, colorF.B);
		}
	}
}