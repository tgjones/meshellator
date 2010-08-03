using System;
using Nexus;

namespace Satis.Core
{
	// ---------------------------------------------------------------------------
	/** A time-value pair specifying a rotation for the given time. 
	 *  Rotations are expressed with quaternions. */
	public class aiQuatKey : IComparable<aiQuatKey>, IEquatable<aiQuatKey>
	{
		/** The time of this key */
		public double mTime;

		/** The value of this key */
		public Quaternion mValue;

		public aiQuatKey()
		{
		}

		/** Construction from a given time and key value */
		public aiQuatKey(double time, Quaternion value)
		{
			mTime = time;
			mValue = value;
		}

		public int CompareTo(aiQuatKey other)
		{
			return mTime.CompareTo(other.mTime);
		}

		public bool Equals(aiQuatKey other)
		{
			return mValue == other.mValue;
		}
	}

	// ---------------------------------------------------------------------------
	/** A time-value pair specifying a certain 3D vector for the given time. */
	public class aiVectorKey : IComparable<aiVectorKey>, IEquatable<aiVectorKey>
	{
		/** The time of this key */
		public double mTime;

		/** The value of this key */
		public Vector3D mValue;

		//! Default constructor
		public aiVectorKey() { }

		//! Construction from a given time and key value
		public aiVectorKey(double time, Vector3D value)
		{
			mTime = time;
			mValue = value;
		}

		public int CompareTo(aiVectorKey other)
		{
			return mTime.CompareTo(other.mTime);
		}

		public bool Equals(aiVectorKey other)
		{
			return mValue == other.mValue;
		}
	}
}