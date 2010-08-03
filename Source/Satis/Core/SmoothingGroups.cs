using System.Collections.Generic;
using Nexus;

namespace Satis.Core
{
	// ---------------------------------------------------------------------------
	/** Helper structure representing a face with smoothing groups assigned */
	public class FaceWithSmoothingGroup
	{
		public FaceWithSmoothingGroup()
		{
			iSmoothGroup = 0;

			mIndices = new uint[3];
			// let the rest uninitialized for performance - in release builds.
			// in debug builds set all indices to a common magic value
#if DEBUG
			mIndices[0] = 0xffffffff;
			mIndices[1] = 0xffffffff;
			mIndices[2] = 0xffffffff;
#endif
		}

		//! Indices. .3ds is using uint16. However, after
		//! an unique vrtex set has been generated it might
		//! be an index becomes > 2^16
		public uint[] mIndices;

		//! specifies to which smoothing group the face belongs to
		public uint iSmoothGroup;
	}

	// ---------------------------------------------------------------------------
/** Helper structure representing a mesh whose faces have smoothing
    groups assigned. This allows us to reuse the code for normal computations 
	from smoothings groups for several loaders (3DS, ASE). All of them 
	use face structures which inherit from #FaceWithSmoothingGroup,
	but as they add extra members and need to be copied by value we
	need to use a template here.
	*/
public class MeshWithSmoothingGroups<T>
{
	//! Vertex positions
	public List<Vector3D> mPositions;

	//! Face lists
	public List<T> mFaces;

	//! List of normal vectors
	public List<Vector3D> mNormals;
};
}