namespace Meshellator.Importers.Autodesk3ds
{
	/**
 * P-key used by the {@link mri.v3ds.PTrack3ds PTrack3ds} class for FOV and Roll.
 *
 * The P-key extends the SplineKey3ds with the <code>P</code> parammeter.
 * P is just an arbitrary name of the single <code>float</code> value that
 * is carried in the key. It is used either for the FOV or Roll tracks used
 * in the {@link mri.v3ds.Camera3ds Camera3ds} class.
 */
	public class PKey3ds : SplineKey3ds
	{
		/**
		 * FOV or Roll value.
		 */
		public float P = 0.0f;
	}
}