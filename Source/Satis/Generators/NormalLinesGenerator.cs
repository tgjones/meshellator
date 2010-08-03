using System;
using Nexus;

namespace Satis.Generators
{
	public static class NormalLinesGenerator
	{
		public static Line3D[] Generate(Mesh mesh)
		{
			Vector3D boundsSize = mesh.Bounds.Size;
			float size = Math.Max(boundsSize.X, Math.Max(boundsSize.Y, boundsSize.Z)) / 50.0f;
			Line3D[] result = new Line3D[mesh.Normals.Count];
			for (int i = 0; i < mesh.Normals.Count; ++i)
				result[i] = new Line3D(mesh.Positions[i], mesh.Positions[i] + (mesh.Normals[i] * size));
			return result;
		}
	}
}