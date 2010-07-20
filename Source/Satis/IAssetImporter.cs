using System.IO;

namespace Satis
{
	public interface IAssetImporter
	{
		Scene ImportFile(FileStream fileStream, string fileName);
	}
}