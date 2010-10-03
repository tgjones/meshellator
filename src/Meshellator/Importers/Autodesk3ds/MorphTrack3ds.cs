namespace Meshellator.Importers.Autodesk3ds
{
	/**
 * Morph track.
 * <br>
 * <br>
 * The morph track controls morphing of a mesh. In 3D Studio, morphing
 * is done by interpolating between the vertices of two or more meshes.
 * All the meshes must have the same origin, i.e. they must have the 
 * same structure and number of vertices.
 */
	public class MorphTrack3ds : Track3ds
	{
		internal MorphKey3ds[] mKey = new MorphKey3ds[0];


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
		public MorphKey3ds key(int i)
		{
			return mKey[i];
		}

		/** 
		 * Access the track (the whole array of keys).
		 *
		 * @return array of keys
		 */
		public MorphKey3ds[] track()
		{
			return mKey;
		}
	}
}