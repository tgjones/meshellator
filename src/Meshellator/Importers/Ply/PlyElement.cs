using System.Collections.Generic;

namespace Meshellator.Importers.Ply
{
	public class PlyElement
	{
		/// <summary>
		/// Element name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Number of elements in this object
		/// </summary>
		public int Num { get; set; }

		/// <summary>
		/// Size of element (bytes) or -1 if variable
		/// </summary>
		public int Size { get; set; }

		/// <summary>
		/// List of properties in the file
		/// </summary>
		public List<PlyProperty> Properties { get; set; }

		public List<PlyElementValue> ElementValues { get; set; }

		public PlyElement()
		{
			Properties = new List<PlyProperty>();
			ElementValues = new List<PlyElementValue>();
		}
	}
}