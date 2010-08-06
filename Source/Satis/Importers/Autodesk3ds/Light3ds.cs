namespace Satis.Importers.Autodesk3ds
{
	/**
 * Light definition.
 */
	public class Light3ds
	{
		// Light name
		internal string mName = "";

		// Light state (on/off)
		internal bool mOff = false;

		// Light position
		internal Vertex3ds mPosition = new Vertex3ds(1.0f, 0.0f, 0.0f);

		// Light target
		internal Vertex3ds mTarget = new Vertex3ds(0.0f, 0.0f, 0.0f);

		// Light color
		internal Color3ds mColor = new Color3ds();

		// Light hotspot
		internal float mHotspot;

		// Light falloff
		internal float mFalloff;

		// light outer range
		internal float mOuterRange;

		// light inner range
		internal float mInnerRange;

		// light multiplexer
		internal float mMultiplexer;

		// light attenuation
		internal float mAttenuation;

		// light roll
		internal float mRoll;

		// light shadowed
		internal bool mShadowed;

		// light shadow bias
		internal float mShadowBias;

		// light shadow filter
		internal float mShadowFilter;

		// light shadow size
		internal float mShadowSize;

		// light cone
		internal bool mCone;

		// light rectangular
		internal bool mRectangular;

		// light aspect
		internal float mAspect;

		// light projector
		internal bool mProjector;

		// light projector name
		internal string mProjectorName;

		// light overshoot
		internal bool mOvershoot;

		// light ray bias
		internal float mRayBias;

		// light ray shadowes
		internal bool mRayShadows;




		/**
		 * Get light name.
		 *
		 * @return light name
		 */
		public string name()
		{
			return mName;
		}

		/**
		 * Get light state.
		 *
		 * @return light state (on/off)
		 */
		public bool off()
		{
			return mOff;
		}

		/**
		 * Get light color.
		 *
		 * @return light color
		 */
		public Color3ds color()
		{
			return mColor;
		}

		/**
		 * Get fixed light position.
		 *
		 * @return fixed position of light
		 */
		public Vertex3ds fixedPosition()
		{
			return mPosition;
		}

		/**
		 * Get fixed light target.
		 *
		 * @return fixed target of light
		 */
		public Vertex3ds fixedTarget()
		{
			return mTarget;
		}

		/**
		 * Get light hotspot.
		 *
		 * @return light hotspot
		 */
		public float hotspot()
		{
			return mHotspot;
		}

		/**
		 * Get light falloff.
		 *
		 * @return light falloff
		 */
		public float falloff()
		{
			return mFalloff;
		}

		/**
		 * Get light outer range.
		 *
		 * @return light outer range
		 */
		public float outerRange()
		{
			return mOuterRange;
		}

		/**
		 * Get light inner range.
		 *
		 * @return light inner range
		 */
		public float innerRange()
		{
			return mInnerRange;
		}

		/**
		 * Get light multiplexer.
		 *
		 * @return light multiplexer
		 */
		public float multiplexer()
		{
			return mMultiplexer;
		}

		/**
		 * Get light attenuation.
		 *
		 * @return light attenuation
		 */
		public float attenuation()
		{
			return mAttenuation;
		}

		/**
		 * Get light roll.
		 *
		 * @return light roll
		 */
		public float roll()
		{
			return mRoll;
		}

		/**
		 * Get light shadowed.
		 *
		 * @return light shadowed
		 */
		public bool shadowed()
		{
			return mShadowed;
		}

		/**
		 * Get light shadow bias.
		 *
		 * @return light shadow bias
		 */
		public float shadowBias()
		{
			return mShadowBias;
		}

		/**
		 * Get light shadow filter.
		 *
		 * @return light shadow filter
		 */
		public float shadowFilter()
		{
			return mShadowFilter;
		}

		/**
		 * Get light shadow size.
		 *
		 * @return light shadow size
		 */
		public float shadowSize()
		{
			return mShadowSize;
		}

		/**
		 * Get light cone.
		 *
		 * @return light cone
		 */
		public bool cone()
		{
			return mCone;
		}

		/**
		 * Get light rectangular.
		 *
		 * @return light rectangular
		 */
		public bool rectangular()
		{
			return mRectangular;
		}

		/**
		 * Get light aspect.
		 *
		 * @return light aspect
		 */
		public float aspect()
		{
			return mAspect;
		}

		/**
		 * Get light projector.
		 *
		 * @return light projector
		 */
		public bool projector()
		{
			return mProjector;
		}

		/**
		 * Get light projector name.
		 *
		 * @return light projector name
		 */
		public string projectorName()
		{
			return mProjectorName;
		}

		/**
		 * Get light overshoot.
		 *
		 * @return light overshoot
		 */
		public bool overshoot()
		{
			return mOvershoot;
		}

		/**
		 * Get light ray bias.
		 *
		 * @return light ray bias
		 */
		public float rayBias()
		{
			return mRayBias;
		}

		/**
		 * Get light ray shadows.
		 *
		 * @return light ray shadows
		 */
		public bool rayShadows()
		{
			return mRayShadows;
		}

	}
}