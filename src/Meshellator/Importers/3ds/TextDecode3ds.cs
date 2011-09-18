using System.Text;

namespace Meshellator.Importers.Autodesk3ds
{
	/**
 * Container class for storing text decode.
 */
	public class TextDecode3ds
	{
		internal StringBuilder mText = new StringBuilder(1024 * 4);

		/**
		 * Clear all text.
		 */
		public void clear()
		{
			mText.Clear();
		}

		/**
		 * Access the text decode.
		 *
		 * @return text string
		 */
		public string text()
		{
			return mText.ToString();
		}
	}
}