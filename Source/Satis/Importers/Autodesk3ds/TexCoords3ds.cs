namespace Satis.Importers.Autodesk3ds
{
	/**
 * Texture mapping U,V coordinate.
 */
	public class TexCoord3ds
	{
		/**
		 * Texture mapping U coordinate.
		 */
		public float U;

		/**
		 * Texture mapping V coordinate.
		 */
		public float V;

		/**
		 * Constructor, initialising the U,V coordinates.
		 */
		public TexCoord3ds(float u, float v)
		{
			U = u;
			V = v;
		}

		/**
		 * Returns a String object representing this TexCoord3ds's value.
		 * 
		 * @return a string representation of this object.
		 */
		public override string ToString()
		{
			return "U V:" +
						 Utils3ds.floatToString(U, 10) +
						 Utils3ds.floatToString(V, 10);
		}
	}
}