namespace Satis.Importers.Autodesk3ds
{
	/**
 * Rotation track.
 * <br>
 * <br>
 * The rotation track control rotation of a mesh around its pivot point.
 * 3D Studio stores the rotation tracks as angular displacements. The first
 * key in the track gives the absolute rotation. The following keys are
 * relative rotations to the previous key. To be useful, the angular 
 * displacements must be converted into quaternions. Using quaternion 
 * multiplication, the absolute rotation at each key can be calculated.
 */
public class RotationTrack3ds : Track3ds
{
	internal RotationKey3ds[] mKey = new RotationKey3ds[0];


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
	public RotationKey3ds key(int i)
	{
		return mKey[i];
	}

	/** 
	 * Access the track (the whole array of keys).
	 *
	 * @return array of keys
	 */
	public RotationKey3ds[] track()
	{
		return mKey;
	}
}
}