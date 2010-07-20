using System.IO;

namespace Satis
{
	public abstract class AssetImporterBase : IAssetImporter
	{
		public abstract Scene ImportFile(FileStream fileStream, string fileName);
	}
}