using System.IO;

namespace Meshellator
{
	public abstract class AssetImporterBase : IAssetImporter
	{
		public abstract Scene ImportFile(FileStream fileStream, string fileName);
	}
}