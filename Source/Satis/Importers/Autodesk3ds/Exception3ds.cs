using System;

namespace Satis.Importers.Autodesk3ds
{
	/**
 * Exception class thrown by the {@link mri.v3ds.Scene3ds Scene3ds} constructors.
 *
 * The exception is thrown in case of I/O and parsing problems. Use 
 * <code>getMessage()</code> to retreive the error message.
 * 
 * @author Mats Byggmästar
 * @version 0.1
 */
	public class Exception3ds : Exception
	{
		/**
		 * 
		 */
		private const long serialVersionUID = 1L;

		public Exception3ds(string message, Exception innerException)
			: base(message, innerException)
		{

		}

		public Exception3ds(string message)
			: base(message)
		{

		}
	}
}