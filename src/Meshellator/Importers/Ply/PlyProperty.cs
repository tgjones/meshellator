namespace Meshellator.Importers.Ply
{
	public class PlyProperty
	{
		/// <summary>
		/// Property name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// File's data type.
		/// </summary>
		public PlyPropertyType ExternalType { get; set; }

		/// <summary>
		/// Program's data type.
		/// </summary>
		public int InternalType { get; set; }

		/// <summary>
		/// Offset bytes of property in a struct.
		/// </summary>
		public int Offset { get; set; }

		/// <summary>
		/// True for list, false for scalar.
		/// </summary>
		public bool IsList { get; set; }

		/// <summary>
		/// File's count type.
		/// </summary>
		public PlyPropertyType CountExternal { get; set; }

		/// <summary>
		/// Program's count type.
		/// </summary>
		public int CountInternal { get; set; }

		/// <summary>
		/// Offset byte for list count.
		/// </summary>
		public int CountOffset { get; set; }
	}
}