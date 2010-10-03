using System.IO;

namespace Meshellator
{
	public interface IAssetImporter
	{
		Scene ImportFile(FileStream fileStream, string fileName);
	}
}