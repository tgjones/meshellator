namespace Satis.Primitives
{
	/// <summary>
	/// Used by BezierShape
	/// </summary>
	public class BezierPatch
	{
		public readonly bool MirrorZ;
		public readonly int[] Indices;

		public BezierPatch(bool mirrorZ, int[] indices)
		{
			//Debug.Assert(indices.Length == 16); // Only true for teapots

			MirrorZ = mirrorZ;
			Indices = indices;
		}
	}
}