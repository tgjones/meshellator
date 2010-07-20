using System.Collections.Generic;
using System.Linq;
using Nexus;
using SlimDX;
using SlimDX.Direct3D9;

namespace Satis.ModelViewer.Services.Direct3D
{
	/// <summary>
	/// Summary description for Model.
	/// </summary>
	public class Model
	{
		#region Variables

		private readonly Device _device;
		private readonly VertexDeclaration _vertexDeclaration;

		#endregion

		public AxisAlignedBoundingBox Bounds { get; set; }
		public List<ModelMesh> Meshes { get; set; }

		#region Constructor

		private Model(Device device, VertexDeclaration vertexDeclaration)
		{
			_device = device;
			_vertexDeclaration = vertexDeclaration;

			Meshes = new List<ModelMesh>();
		}

		#endregion

		#region Methods

		public static Model FromScene(Scene scene, Device device)
		{
			VertexDeclaration vertexDeclaration = new VertexDeclaration(device,
				VertexPositionNormalTexture.VertexElements);
			Model result = new Model(device, vertexDeclaration);
			AxisAlignedBoundingBox modelBounds = new AxisAlignedBoundingBox();
			foreach (Mesh mesh in scene.Meshes)
			{
				VertexBuffer vertexBuffer = new VertexBuffer(device,
					mesh.Positions.Count * VertexPositionNormalTexture.SizeInBytes,
					Usage.WriteOnly, VertexFormat.None, Pool.Default);
				DataStream vertexDataStream = vertexBuffer.Lock(0,
					mesh.Positions.Count * VertexPositionNormalTexture.SizeInBytes,
					LockFlags.None);
				VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[mesh.Positions.Count];
				for (int i = 0; i < vertices.Length; ++i)
					vertices[i] = new VertexPositionNormalTexture(mesh.Positions[i], Vector3D.Up, Point2D.Zero);
				vertexDataStream.WriteRange(vertices);
				vertexBuffer.Unlock();

				IndexBuffer indexBuffer = new IndexBuffer(device, mesh.Indices.Count * sizeof(int),
					Usage.WriteOnly, Pool.Default, false);
				DataStream indexDataStream = indexBuffer.Lock(0, mesh.Indices.Count * sizeof(int), LockFlags.None);
				indexDataStream.WriteRange(mesh.Indices.ToArray());
				indexBuffer.Unlock();

				Effect effect = Effect.FromFile(device, @"Services\Direct3D\Effect.fx", ShaderFlags.None);

				AxisAlignedBoundingBox bounds = new AxisAlignedBoundingBox(mesh.Positions);
				modelBounds = AxisAlignedBoundingBox.Union(modelBounds, bounds);

				ModelMesh modelMesh = new ModelMesh(device, vertexBuffer,
					mesh.Positions.Count, indexBuffer, mesh.Indices.Count / 3,
					effect, Matrix.Identity, scene.Materials.Single(m => m.Name == mesh.MaterialName),
					bounds);
				result.Meshes.Add(modelMesh);
			}
			result.Bounds = modelBounds;
			return result;
		}

		public void Draw(Matrix viewProjection, Vector3D eyePosition)
		{
			_device.VertexDeclaration = _vertexDeclaration;

			foreach (ModelMesh modelMesh in Meshes)
				modelMesh.Draw(viewProjection, eyePosition);
		}

		/*private void ApplyMaterial(BasicEffect basicEffect, Material material)
		{
			basicEffect.DiffuseColor = material.DiffuseColor.ToVector3();
			basicEffect.SpecularColor = material.SpecularColor.ToVector3();
		}*/

		#endregion
	}

	public class Subset
	{
		public int FaceStart;
		public int FaceCount;
		public Material Material;
		//public Texture2D Texture;
	}
}