using NUnit.Framework;

namespace Meshellator.Tests.Importers.Autodesk3ds
{
	[TestFixture]
	public class Autodesk3dsTest
	{
		[Test]
		public void CanImport3dsFile()
		{
			Scene scene = MeshellatorLoader.ImportFromFile(@"Models\3ds\85-nissan-fairlady.3ds");
		}
	}
}