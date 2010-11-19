using Nexus;

namespace Meshellator.Util
{
	public static class MeshUtility
	{
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