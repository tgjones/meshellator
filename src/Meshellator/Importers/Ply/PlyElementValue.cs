using System.Collections.Generic;

namespace Meshellator.Importers.Ply
{
	public class PlyElementValue
	{
		public List<object> PropertyValues { get; set; }

		public PlyElementValue()
		{
			PropertyValues = new List<object>();
		}
	}
}