using System.IO;
using NUnit.Framework;

//using Satis.Importers.Autodesk3ds;

namespace Meshellator.Tests
{
	[TestFixture]
	public class Autodesk3dsTest
	{
		[Test]
		public void CanImport3dsFile()
		{
			Scene scene = Meshellator.ImportFromFile(@"Models\3ds\85-nissan-fairlady.3ds");
		}
	}
}