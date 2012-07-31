using Nexus;
using Nexus.Graphics.Colors;

namespace Meshellator.Importers.Autodesk3ds
{
	/**
 * Color definition.
 */
	public class Color3ds
	{

		internal float mRed;
		internal float mGreen;
		internal float mBlue;

		/**
		 * Constructor, initialising the red, green and blue components.
		 */
		public Color3ds()
		{
			mRed = 1.0f;
			mGreen = 1.0f;
			mBlue = 1.0f;
		}

		/**
		 * Constructor, initialising the red, green and blue components.
		 */
		public Color3ds(float r, float g, float b)
		{
			mRed = r;
			mGreen = g;
			mBlue = b;
		}

		/**
		 * Get red component.
		 *
		 * @return red component
		 */
		public float red()
		{
			return mRed;
		}

		/**
		 * Get green component.
		 *
		 * @return green component
		 */
		public float green()
		{
			return mGreen;
		}

		/**
		 * Get blue component.
		 *
		 * @return blue component
		 */
		public float blue()
		{
			return mBlue;
		}

		public ColorRgbF ToColorRgbF()
		{
			return new ColorRgbF(red(), green(), blue());
		}
	}
}