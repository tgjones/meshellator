namespace Satis.Importers.Autodesk3ds
{
	/**
 * X,Y,Z vertex.
 */
	public class Vertex3ds
	{
		/**
		 * X coordinate.
		 */
		public float X;

		/**
		 * Y coordinate.
		 */
		public float Y;

		/**
		 * Z coordinate.
		 */
		public float Z;

		/**
		 * Constructor, initialising the Z,Y,Z coordinates.
		 */
		public Vertex3ds(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		/**
		 * Returns a String object representing this Vertex3ds's value.
		 * 
		 * @return a string representation of this object.
		 */
		public string toString()
		{
			return "X Y Z:" +
						 Utils3ds.floatToString(X, 14) +
						 Utils3ds.floatToString(Y, 14) +
						 Utils3ds.floatToString(Z, 14);
		}
	}
}