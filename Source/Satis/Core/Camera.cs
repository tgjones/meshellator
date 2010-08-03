using Nexus;

namespace Satis.Core
{
	// ---------------------------------------------------------------------------
	/** Helper structure to describe a virtual camera. 
	 *
	 * Cameras have a representation in the node graph and can be animated.
	 * An important aspect is that the camera itself is also part of the
	 * scenegraph. This means, any values such as the look-at vector are not 
	 * *absolute*, they're <b>relative</b> to the coordinate system defined
	 * by the node which corresponds to the camera. This allows for camera
	 * animations. For static cameras parameters like the 'look-at' or 'up' vectors
	 * are usually specified directly in aiCamera, but beware, they could also
	 * be encoded in the node transformation. The following (pseudo)code sample 
	 * shows how to do it: <br><br>
	 * @code
	 * // Get the camera matrix for a camera at a specific time
	 * // if the node hierarchy for the camera does not contain
	 * // at least one animated node this is a static computation
	 * get-camera-matrix (node sceneRoot, camera cam) : matrix
	 * {
	 *    node   cnd = find-node-for-camera(cam)
	 *    matrix cmt = identity()
	 *
	 *    // as usual - get the absolute camera transformation for this frame
	 *    for each node nd in hierarchy from sceneRoot to cnd
	 *      matrix cur
	 *      if (is-animated(nd))
	 *         cur = eval-animation(nd)
	 *      else cur = nd->mTransformation;
	 *      cmt = mult-matrices( cmt, cur )
	 *    end for
	 *
	 *    // now multiply with the camera's own local transform
	 *    cam = mult-matrices (cam, get-camera-matrix(cmt) )
	 * }
	 * @endcode
	 *
	 * @note some file formats (such as 3DS, ASE) export a "target point" -
	 * the point the camera is looking at (it can even be animated). Assimp
	 * writes the target point as a subnode of the camera's main node,
	 * called "<camName>.Target". However this is just additional information
	 * then the transformation tracks of the camera main node make the
	 * camera already look in the right direction.
	 * 
	*/
	public class aiCamera
	{
		/** The name of the camera.
		 *
		 *  There must be a node in the scenegraph with the same name.
		 *  This node specifies the position of the camera in the scene
		 *  hierarchy and can be animated.
		 */
		public string mName;

		/** Position of the camera relative to the coordinate space
		 *  defined by the corresponding node.
		 *
		 *  The default value is 0|0|0.
		 */
		public Point3D mPosition;


		/** 'Up' - vector of the camera coordinate system relative to
		 *  the coordinate space defined by the corresponding node.
		 *
		 *  The 'right' vector of the camera coordinate system is
		 *  the cross product of  the up and lookAt vectors.
		 *  The default value is 0|1|0. The vector
		 *  may be normalized, but it needn't.
		 */
		public Vector3D mUp;


		/** 'LookAt' - vector of the camera coordinate system relative to
		 *  the coordinate space defined by the corresponding node.
		 *
		 *  This is the viewing direction of the user.
		 *  The default value is 0|0|1. The vector
		 *  may be normalized, but it needn't.
		 */
		public Vector3D mLookAt;


		/** Half horizontal field of view angle, in radians. 
		 *
		 *  The field of view angle is the angle between the center
		 *  line of the screen and the left or right border.
		 *  The default value is 1/4PI.
		 */
		public float mHorizontalFOV;

		/** Distance of the near clipping plane from the camera.
		 *
		 * The value may not be 0.f (for arithmetic reasons to prevent
		 * a division through zero). The default value is 0.1f.
		 */
		public float mClipPlaneNear;

		/** Distance of the far clipping plane from the camera.
		 *
		 * The far clipping plane must, of course, be further away than the
		 * near clipping plane. The default value is 1000.f. The ratio
		 * between the near and the far plane should not be too
		 * large (between 1000-10000 should be ok) to avoid floating-point
		 * inaccuracies which could lead to z-fighting.
		 */
		public float mClipPlaneFar;


		/** Screen aspect ratio.
		 *
		 * This is the ration between the width and the height of the
		 * screen. Typical values are 4/3, 1/2 or 1/1. This value is
		 * 0 if the aspect ratio is not defined in the source file.
		 * 0 is also the default value.
		 */
		public float mAspect;

		public aiCamera()
		{
			mUp = Vector3D.Up;
			mLookAt = Vector3D.Backward;
			mHorizontalFOV = MathUtility.PI_OVER_4;
			mClipPlaneNear = 0.1f;
			mClipPlaneFar = 1000;
			mAspect = 0;
		}

		/** @brief Get a *right-handed* camera matrix from me
		 *  @param out Camera matrix to be filled
		 */
		public Matrix3D GetCameraMatrix()
		{
			return Matrix3D.CreateLookAt(mPosition, mLookAt, mUp);
		}
	}
}