namespace Meshellator.Importers.Autodesk3ds
{
	/**
 * Position or scaling track.
 * <br>
 * <br>
 * If used as position track, the track controls the position of a
 * mesh or camera (target or position). If used as scaling track, the
 * track controls the scaling of a mesh.
 */
	public class XYZTrack3ds : Track3ds
	{
		internal XYZKey3ds[] mKey = new XYZKey3ds[0];


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
		public XYZKey3ds key(int i)
		{
			return mKey[i];
		}

		/** 
		 * Access the track (the whole array of keys).
		 *
		 * @return array of keys
		 */
		public XYZKey3ds[] track()
		{
			return mKey;
		}
	}
}