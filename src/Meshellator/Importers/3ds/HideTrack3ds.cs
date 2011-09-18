namespace Meshellator.Importers.Autodesk3ds
{
	/**
 * Hide track.
 * <br>
 * <br>
 * The hide track controls when meshes should be visible or
 * hidden.
 */
	public class HideTrack3ds : Track3ds
	{
		// Array of hide keys
		internal HideKey3ds[] mKey = new HideKey3ds[0];


		/**
		 * Get number of keys.
		 *
		 * @return number of keys
		 */
		public int keys()
		{
			return mKey.Length;
		}

		/**
		 * Access a specific key.
		 *
		 * @param i index into key array [0 ... keys()-1]
		 * @return the specified key
		 */
		public HideKey3ds key(int i)
		{
			return mKey[i];
		}

		/** 
		 * Access the track (the whole array of keys).
		 *
		 * @return array of keys
		 */
		public HideKey3ds[] track()
		{
			return mKey;
		}
	}
}