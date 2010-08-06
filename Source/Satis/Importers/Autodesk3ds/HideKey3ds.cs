namespace Satis.Importers.Autodesk3ds
{
	/**
 * Hide key used by the {@link mri.v3ds.HideTrack3ds HideTrack3ds} class.
 *
 * The hide key only contains the <code>Frame</code> number at which the
 * visibility flag is toggled. I.e. a visible mesh is hidden and a hidden
 * mesh becomes visible.
 */
	public class HideKey3ds
	{
		/**
		 * Frame number where the visibility flag toggles.
		 */
		public int Frame = 0;
	}
}