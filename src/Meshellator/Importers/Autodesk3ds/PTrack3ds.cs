namespace Meshellator.Importers.Autodesk3ds
{
	/**
 * Class used for FOV or Roll tracks in a camera.
 */
public class PTrack3ds : Track3ds
{
	internal PKey3ds[] mKey = new PKey3ds[0];


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
	public PKey3ds key(int i)
	{
		return mKey[i];
	}

	/** 
	 * Access the track (the whole array of keys).
	 *
	 * @return array of keys
	 */
	public PKey3ds[] track()
	{
		return mKey;
	}

}
}