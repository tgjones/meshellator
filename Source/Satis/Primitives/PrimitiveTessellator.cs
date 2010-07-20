namespace Satis.Primitives
{
	public abstract class PrimitiveTessellator : BasicPrimitiveTessellator
	{
		protected int TessellationLevel { get; set; }

		protected PrimitiveTessellator(int tessellationLevel)
		{
			TessellationLevel = tessellationLevel;
		}
	}
}