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

		[Test]
		public void CanImport3dsFileWithTextures()
		{
			// Act.
			Scene scene = MeshellatorLoader.ImportFromFile(@"Models\3ds\Face\Jane_solid_3ds.3ds");

			// Assert.
			Assert.That(scene.Materials, Has.Count.EqualTo(3));
			Assert.That(scene.Materials[0].DiffuseTextureName, Is.EqualTo(@"Models\3ds\Face\Jane_so1.jpg"));
			Assert.That(scene.Materials[1].DiffuseTextureName, Is.EqualTo(@"Models\3ds\Face\Jane_so2.jpg"));
			Assert.That(scene.Materials[2].DiffuseTextureName, Is.EqualTo(@"Models\3ds\Face\Jane_sol.jpg"));
		}
	}
}