using System.Collections.Generic;

namespace Meshellator.Importers.Ply
{
	public class PlyFile
	{
		public PlyFileType FileType { get; set; }
		public float Version { get; set; }
		public List<PlyElement> Elements { get; set; }
		public List<string> Comments { get; set; }
		public List<string> ObjectInformationItems { get; set; }
		//PlyElement *which_elem;       /* which element we're currently writing */
		//PlyOtherElems *other_elems;   /* "other" elements from a PLY file */

		public PlyFile()
		{
			Elements = new List<PlyElement>();
			Comments = new List<string>();
			ObjectInformationItems = new List<string>();
		}
	}
}