namespace Satis.Importers.Autodesk3ds
{
	/// <summary>
	///  * Camera object in a 3D vector animation. 
	/// The Camera3ds class contains the following data that is read 
	/// from the 3D-editor part of the 3ds-file:<br>
	/// - Camera name<br>
	/// - Fixed camera position<br>
	/// - Fixed camera target position<br>
	/// - Fixed roll angle<br>
	/// - Fixed lens<br>
	/// - Distance to near- and far-plane<br>
	/// <br>
	/// The following data is from the keyframer:<br>
	/// - Target node id in the object hierarchy<br>
	/// - Target parent node id in the object hierarchy<br>
	/// - Target node flags<br>
	/// - Position node id in the object hierarchy<br>
	/// - Position parent node id in the object hierarchy<br>
	/// - Position node flags<br>
	/// - Position spline track<br>
	/// - Target spline track<br>
	/// - FOV spline track<br>
	/// - Roll spline track<br>
	/// <br>
	/// The "fixed" parameters that are loaded from the 3D-editor part
	/// of the 3ds-file are not needed during an animation as the keyframer 
	/// data provides the same information, in the corresponding tracks.
	/// </summary>
	public class Camera3ds
	{
		// Camera name
		internal string mName = "";

		// Camera fixed position
		internal Vertex3ds mPosition;	// = new Vertex3ds(1.0f, 0.0f, 0.0f);

		// Camera fixed target
		internal Vertex3ds mTarget;	// = new Vertex3ds(0.0f, 0.0f, 0.0f);

		// Camera fixed roll angle in degrees
		internal float mRoll;

		// Camera lens
		internal float mLens;

		// Near plane
		internal float mNearPlane;

		// Far plane
		internal float mFarPlane;


		// Keyframer parameters:

		// Target node id
		internal int mTargetNodeId;

		// Target parent node id
		internal int mTargetParentNodeId;

		// Target node flags
		internal int mTargetNodeFlags;

		// Position node id
		internal int mPositionNodeId;

		// Position parent node id
		internal int mPositionParentNodeId;

		// Position node flags
		internal int mPositionNodeFlags;

		// Position spline track
		internal XYZTrack3ds mPositionTrack = new XYZTrack3ds();

		// Target spline track
		internal XYZTrack3ds mTargetTrack = new XYZTrack3ds();

		// FOV spline track
		internal PTrack3ds mFovTrack = new PTrack3ds();

		// Roll spline track
		internal PTrack3ds mRollTrack = new PTrack3ds();


		public Camera3ds()
		{
			// Camera fixed position
			mPosition = new Vertex3ds(1.0f, 0.0f, 0.0f);

			// Camera fixed target
			mTarget = new Vertex3ds(0.0f, 0.0f, 0.0f);
		}



		/** 
		 * Get camera name.
		 *
		 * @return name of camera
		 */
		public string name()
		{
			return mName;
		}



		/**
		 * Get fixed camera position.
		 *
		 * @return fixed position of camera
		 */
		public Vertex3ds fixedPosition()
		{
			return mPosition;
		}



		/**
		 * Get fixed camera target.
		 *
		 * @return fixed target of camera
		 */
		public Vertex3ds fixedTarget()
		{
			return mTarget;
		}



		/**
		 * Get fixed camera roll angle in degrees.
		 *
		 * @return fixed roll angle of camera
		 */
		public float fixedRoll()
		{
			return mRoll;
		}



		/**
		 * Get fixed camera lens.
		 *
		 * @return fixed camera lens
		 */
		public float fixedLens()
		{
			return mLens;
		}



		/**
		 * Get distance to camera near-plane.
		 *
		 * @return distance to camera near-plane
		 */
		public float nearPlane()
		{
			return mNearPlane;
		}



		/**
		 * Get distance to camera far-plane.
		 *
		 * @return distance to camera far-plane
		 */
		public float farPlane()
		{
			return mFarPlane;
		}



		/**
		 * Get target node id.
		 *
		 * @return target node id
		 */
		public int targetNodeId()
		{
			return mTargetNodeId;
		}



		/**
		 * Get target parent node id.
		 *
		 * @return target parent node id
		 */
		public int targetParentNodeId()
		{
			return mTargetParentNodeId;
		}

		/**
		 * Get target node flags.
		 *
		 * @return target node flags
		 */
		public int targetNodeFlags()
		{
			return mTargetNodeFlags;
		}

		/**
		 * Get position node id.
		 *
		 * @return position node id
		 */
		public int positionNodeId()
		{
			return mPositionNodeId;
		}

		/**
		 * Get position parent node id.
		 *
		 * @return position parent node id
		 */
		public int positionParentNodeId()
		{
			return mPositionParentNodeId;
		}

		/**
		 * Get position node flags.
		 *
		 * @return position node flags
		 */
		public int positionNodeFlags()
		{
			return mPositionNodeFlags;
		}

		/**
		 * Access position spline track.
		 *
		 * @return position spline track
		 */
		public XYZTrack3ds position()
		{
			return mPositionTrack;
		}

		/**
		 * Access target spline track.
		 *
		 * @return target spline track
		 */
		public XYZTrack3ds target()
		{
			return mTargetTrack;
		}

		/**
		 * Access FOV spline track.
		 *
		 * @return FOV spline track
		 */
		public PTrack3ds fov()
		{
			return mFovTrack;
		}

		/**
		 * Access roll spline track.
		 *
		 * @return roll spline track
		 */
		public PTrack3ds roll()
		{
			return mRollTrack;
		}
	}
}