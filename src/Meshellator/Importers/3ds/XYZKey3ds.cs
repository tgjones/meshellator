namespace Meshellator.Importers.Autodesk3ds
{
	/**
 * X,Y,Z-key used by the {@link mri.v3ds.XYZTrack3ds XYZTrack3ds} class used for 
 * position and scaling.
 *
 * The XYZKey3ds extends the SplineKey3ds with the <code>X, Y, Z</code>
 * parameters. X,Y,Z is either position or scaling depending of the usage.
 */
	public class XYZKey3ds : SplineKey3ds
	{
		/**
		 * X position or scaling.
		 */
		public float X = 0.0f;

		/**
		 * Y position or scaling.
		 */
		public float Y = 0.0f;

		/**
		 * Z position or scaling.
		 */
		public float Z = 0.0f;
	}
}