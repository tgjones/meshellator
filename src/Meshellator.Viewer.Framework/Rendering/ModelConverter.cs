using System;
using Nexus;
using SlimDX;
using SlimDX.Direct3D9;

namespace Meshellator.Viewer.Framework.Rendering
{
	public class ModelConverter
	{
		public static Model FromScene(Scene scene, Device device)
		{
			VertexDeclaration vertexDeclaration = new VertexDeclaration(device,
				VertexPositionNormalTexture.VertexElements);
			Model result = new Model(scene, device, vertexDeclaration);
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
					vertices[i] = new VertexPositionNormalTexture(mesh.Positions[i], mesh.Normals[i], Point2D.Zero);
				vertexDataStream.WriteRange(vertices);
				vertexBuffer.Unlock();

				IndexBuffer indexBuffer = new IndexBuffer(device, mesh.Indices.Count * sizeof(int),
					Usage.WriteOnly, Pool.Default, false);
				DataStream indexDataStream = indexBuffer.Lock(0, mesh.Indices.Count * sizeof(int), LockFlags.None);
				indexDataStream.WriteRange(mesh.Indices.ToArray());
				indexBuffer.Unlock();

				ModelMesh modelMesh = new ModelMesh(mesh, device, vertexBuffer,
					mesh.Positions.Count, indexBuffer, mesh.PrimitiveCount,
					Matrix3D.Identity, mesh.Material,
					GetPrimitiveType(mesh.PrimitiveTopology));
				result.Meshes.Add(modelMesh);
			}
			return result;
		}

		private static PrimitiveType GetPrimitiveType(PrimitiveTopology primitiveTopology)
		{
			switch (primitiveTopology)
			{
				case PrimitiveTopology.TriangleList :
					return PrimitiveType.TriangleList;
				case PrimitiveTopology.TriangleStrip :
					return PrimitiveType.TriangleStrip;
				default :
					throw new NotSupportedException();
			}
		}
	}
}