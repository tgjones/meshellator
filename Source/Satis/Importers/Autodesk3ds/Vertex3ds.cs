namespace Satis.Importers.Autodesk3ds
{
	/**
 * X,Y,Z vertex.
 */
	public class Vertex3ds
	{
		/// <summary>
		/// X coordinate.
		/// </summary>
		public float X;

		/// <summary>
		/// Y coordinate.
		/// </summary>
		public float Y;

		/// <summary>
		/// Z coordinate.
		/// </summary>
		public float Z;

		/// <summary>
		/// Constructor, initialising the X,Y,Z coordinates.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public Vertex3ds(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public override string ToString()
		{
			return "X Y Z:" +
						 Utils3ds.floatToString(X, 14) +
						 Utils3ds.floatToString(Y, 14) +
						 Utils3ds.floatToString(Z, 14);
		}
	}
}