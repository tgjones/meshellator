namespace Satis.Importers.Autodesk3ds
{
	/**
 * Material selector for the faces in a mesh.
 * <br>
 * <br>
 * A single mesh (class {@link mri.v3ds.Mesh3ds Mesh3ds}) usually consists
 * of many faces (class {@link mri.v3ds.Face3ds Face3ds}). The faces can have 
 * different materials. The <code>FaceMat3ds</code> class is used to
 * assign a material to a set of faces. If the mesh uses several materials,
 * the mesh have one <code>FaceMat3ds</code> object for each material.
 * <br>
 * <br>
 * The <code>FaceMat3ds</code> class consists of a material index which selects
 * the material from the global material array in the 
 * {@link mri.v3ds.Scene3ds Scene3ds} class. It also contains an array of face
 * indexes which specify which faces in the mesh face array, this material should
 * be assigned to.
 * <br>
 * <br>
 * Assigning materials to faces in this way is very handly when rendering
 * a mesh using a HW-accelerated 3D-engine. Using an 3D-accelerator, it is 
 * efficient to render as many faces as possible using the same material. 
 * Switching material is typically a slow process.
 */
	public class FaceMat3ds
	{
		// Which material (index into material array in Scene3ds class)
		internal int mMatIndex;

		// Array of faces indexes using this material
		internal int[] mFaceIndex = new int[0];


		/**
		 * Get material number. This is an index into global material array in the
		 * {@link mri.v3ds.Scene3ds Scene3ds} class.
		 *
		 * @return material number
		 */
		public int material()
		{
			return mMatIndex;
		}

		/**
		 * Get number of face indexes.
		 *
		 * @return number of face indexes
		 */
		public int faces()
		{
			return mFaceIndex.Length;
		}

		/**
		 * Access a specific face index in the face index array.
		 *
		 * @param i index into face index array [0 ... faces()-1]
		 * @return the specified face index
		 */
		public int face(int i)
		{
			return mFaceIndex[i];
		}

		/** 
		 * Access the whole array of face indexes.
		 *
		 * @return array of face indexes
		 */
		public int[] faceArray()
		{
			return mFaceIndex;
		}
	}
}