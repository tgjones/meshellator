using System;
using System.Linq;
using Nexus;

namespace Meshellator
{
	public static class MeshUtility
	{
		public static void CalculateNormals(Mesh mesh, bool overwriteExistingNormals)
		{
			if (mesh == null)
				throw new ArgumentNullException("mesh");

			if (overwriteExistingNormals || !mesh.Normals.Any())
			{
				Vector3D[] vertexNormals = new Vector3D[mesh.Positions.Count];
				AccumulateTriangleNormals(mesh.Indices, mesh.Positions, vertexNormals);
				for (int i = 0; i < vertexNormals.Length; i++)
					vertexNormals[i] = Vector3D.Normalize(vertexNormals[i]);
				mesh.Normals.AddRange(vertexNormals);
			}
		}

		private static void AccumulateTriangleNormals(Int32Collection indices, Point3DCollection positions, Vector3D[] vertexNormals)
		{
			for (int i = 0; i < indices.Count; i += 3)
			{
				Point3D vector4 = positions[indices[i]];
				Point3D vector = positions[indices[i + 1]];
				Point3D vector3 = positions[indices[i + 2]];
				Vector3D vector2 = Vector3D.Normalize(Vector3D.Cross(vector3 - vector, vector - vector4));
				for (int j = 0; j < 3; j++)
					vertexNormals[indices[i + j]] += vector2;
			}
		}

		public static Int32Collection ConvertTriangleStripToTriangleList(Int32Collection indices)
		{
			Int32Collection newIndices = new Int32Collection();
			for (int i = 2; i < indices.Count; ++i)
			{
				if (i % 2 == 0)
				{
					newIndices.Add(indices[i - 2]);
					newIndices.Add(indices[i - 1]);
					newIndices.Add(indices[i - 0]);
				}
				else
				{
					newIndices.Add(indices[i - 1]);
					newIndices.Add(indices[i - 2]);
					newIndices.Add(indices[i - 0]);
				}
			}
			return newIndices;
		}
	}
}