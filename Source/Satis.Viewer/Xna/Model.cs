using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Satis.Viewer.Xna
{
	/// <summary>
	/// Summary description for Model.
	/// </summary>
	public class Model : IRenderable
	{
		#region Variables

		private VertexBuffer m_pVertexBuffer;
		private VertexDeclaration m_pVertexDeclaration;
		private int m_nNumVertices;
		private IndexBuffer m_pIndexBuffer;
		private Subset[] m_pSubsets;
		private GraphicsDevice m_pDevice;

		#endregion

		#region Constructor

		public Model(GraphicsDevice pDevice, VertexBuffer pVertexBuffer, int nNumVertices,
			VertexDeclaration pVertexDeclaration, IndexBuffer pIndexBuffer, Subset[] pSubsets)
		{
			m_pVertexBuffer = pVertexBuffer;
			m_nNumVertices = nNumVertices;
			m_pVertexDeclaration = pVertexDeclaration;
			m_pIndexBuffer = pIndexBuffer;
			m_pSubsets = pSubsets;
			m_pDevice = pDevice;
		}

		#endregion

		#region Methods

		public void Render(BasicEffect basicEffect)
		{
			m_pDevice.VertexDeclaration = m_pVertexDeclaration;

			m_pDevice.Vertices[0].SetSource(m_pVertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
			m_pDevice.Indices = m_pIndexBuffer;

			basicEffect.Begin();
			for (int i = 0; i < m_pSubsets.Length; i++)
			{
				ApplyMaterial(basicEffect, m_pSubsets[i].Material);
				if (m_pSubsets[i].Texture != null)
				{
					basicEffect.Texture = m_pSubsets[i].Texture;
					basicEffect.TextureEnabled = true;
				}
				else
				{
					basicEffect.TextureEnabled = false;
				}

				foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
				{
					pass.Begin();
					m_pDevice.DrawIndexedPrimitives(
							PrimitiveType.TriangleList, 0, 0, m_nNumVertices,
							m_pSubsets[i].FaceStart, m_pSubsets[i].FaceCount);
					pass.End();
				}
			}
			basicEffect.End();
		}

		private void ApplyMaterial(BasicEffect basicEffect, Material material)
		{
			basicEffect.DiffuseColor = material.DiffuseColor.ToVector3();
			basicEffect.SpecularColor = material.SpecularColor.ToVector3();
		}

		#endregion
	}

	public static class VectorExtensions
	{
		public static Vector3 ToVector3(this Nexus.Color color)
		{
			Nexus.ColorF temp = (Nexus.ColorF) color;
			return new Vector3(temp.R, temp.G, temp.B);
		}

		public static Vector3 ToVector3(this Nexus.Point3D point)
		{
			return new Vector3(point.X, point.Y, point.Z);
		}

		public static Vector2 ToVector2(this Nexus.Point3D point)
		{
			return new Vector2(point.X, point.Y);
		}
	}

	public class Subset
	{
		public int FaceStart;
		public int FaceCount;
		public Material Material;
		public Texture2D Texture;
	}
}