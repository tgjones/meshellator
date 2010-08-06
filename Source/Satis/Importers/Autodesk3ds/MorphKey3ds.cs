namespace Satis.Importers.Autodesk3ds
{
	/**
 * Morph key used by the {@link mri.v3ds.MorphTrack3ds MorphTrack3ds} class.
 *
 * The morph key extends the SplineKey3ds with the <code>Mesh</code>
 * parameter. The Mesh parameter simply identifies a mesh in the global 
 * mesh array, accessible from the {@link mri.v3ds.Scene3ds Scene3ds} class.
 */
public class MorphKey3ds : SplineKey3ds
{
	/**
	 * Mesh number.
	 */
	public int Mesh = 0;
}
}