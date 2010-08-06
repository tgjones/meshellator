namespace Satis.Importers.Autodesk3ds
{
	/**
 * Rotation key used by the {@link mri.v3ds.RotationTrack3ds RotationTrack3ds} class.
 *
 * The rotation key extends the SplineKey3ds with the <code>A, X, Y, Z</code>
 * parameters, the angular displacement. A is the rotation angle and X,Y,Z is
 * the rotation axis. To be useful for interpolation, the keys must be 
 * converted into quaternions.
 */
	public class RotationKey3ds : SplineKey3ds
	{
		/**
		 * Rotartion angle.
		 */
		public float A = 0.0f;

		/**
		 * X component of rotation axis.
		 */
		public float X = 1.0f;

		/**
		 * Y component of rotation axis.
		 */
		public float Y = 0.0f;

		/**
		 * Z component of rotation axis.
		 */
		public float Z = 0.0f;
	}
}